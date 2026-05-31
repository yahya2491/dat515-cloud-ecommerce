using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            var problem = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment() ? ex.Message : null,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = problem.Status ?? 500;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
