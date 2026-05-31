using Ecommerce.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text.Json.Serialization;
using Ecommerce.Api.Services;
using Stripe;
using Ecommerce.Api.Health;

var builder = WebApplication.CreateBuilder(args);

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("AdminKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "X-Admin-Key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Admin API key for accessing administrative endpoints"
    });

    c.OperationFilter<Ecommerce.Api.Swagger.AdminKeyOperationFilter>();
    c.OperationFilter<Ecommerce.Api.Swagger.PaymentExampleOperationFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.Scan(scan => scan
    .FromAssemblyOf<IProductService>()
    .AddClasses(classes => classes.InNamespaces("Ecommerce.Api.Services"))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

builder.Services.AddHttpClient("stripe-check");

var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory")
                 || string.Equals(Environment.GetEnvironmentVariable("USE_INMEMORY_DB"), "true", StringComparison.OrdinalIgnoreCase);
var enableDbRetry = builder.Configuration.GetValue<bool>("EnableDbRetry")
                    || string.Equals(Environment.GetEnvironmentVariable("ENABLE_DB_RETRY"), "true", StringComparison.OrdinalIgnoreCase);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (useInMemory)
    {
        options.UseInMemoryDatabase("ecommerce-inmemory");
    }
    else
    {
        if (enableDbRetry)
        {
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                o => o.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorCodesToAdd: null)
            );
        }
        else
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        }
    }
});

builder.Services.AddHealthChecks();
builder.Services.AddHealthChecks()
    .AddCheck<AppDbContextHealthCheck>("database")
    .AddCheck<StripeHealthCheck>("stripe");
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Ecommerce.Api.Validators.ProductCreateDtoValidator>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("[CORS] Configured to allow all origins for cluster deployment");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    var origin = context.Request.Headers["Origin"].FirstOrDefault();
    logger.LogInformation("[CORS] Request from origin: {Origin}, Method: {Method}, Path: {Path}", origin ?? "null", context.Request.Method, context.Request.Path);

    await next();

    var allowOrigin = context.Response.Headers["Access-Control-Allow-Origin"].FirstOrDefault();
    logger.LogInformation("[CORS] Response headers - Access-Control-Allow-Origin: {AllowOrigin}", allowOrigin ?? "null");
});

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseMiddleware<Ecommerce.Api.Middleware.ExceptionMiddleware>();
app.UseHttpMetrics();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/healthz");
app.MapHealthChecks("/ready");
app.MapMetrics();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var providerName = db.Database.ProviderName;

    logger.LogInformation("[Startup] EF Provider: {Provider} (UseInMemory={UseInMemory}, EnableDbRetry={EnableDbRetry})", providerName, useInMemory, enableDbRetry);

    if (db.Database.IsRelational())
    {
        var maxAttempts = 10;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await db.Database.MigrateAsync();
                break;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                var delay = TimeSpan.FromSeconds(Math.Min(30, attempt * 2));
                logger.LogWarning(ex, "[Startup] Migration attempt {Attempt} failed: {Message}. Retrying in {DelaySeconds}s...", attempt, ex.Message, delay.TotalSeconds);
                await Task.Delay(delay);
            }
        }
    }
    else
    {
        await db.Database.EnsureCreatedAsync();
    }

    await SeedData.EnsureSeedDataAsync(db);
}

app.Run();

public partial class Program { }
