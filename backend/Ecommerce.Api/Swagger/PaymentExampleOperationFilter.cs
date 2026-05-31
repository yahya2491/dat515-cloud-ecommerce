using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Ecommerce.Api.Swagger;

public class PaymentExampleOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Only apply to POST checkout operations
        var relativePath = context.ApiDescription.RelativePath ?? string.Empty;
        var httpMethod = context.ApiDescription.HttpMethod ?? string.Empty;

        if (!httpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase)) return;
        if (!relativePath.Contains("checkout", StringComparison.OrdinalIgnoreCase)) return;

        operation.RequestBody ??= new OpenApiRequestBody();
        operation.RequestBody.Content ??= new Dictionary<string, OpenApiMediaType>();

        // Create a schema that reflects the expected request body (no UserId)
        var schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["amount"] = new OpenApiSchema { Type = "integer", Format = "int64", Description = "Amount in smallest currency unit" },
                ["currency"] = new OpenApiSchema { Type = "string" },
                ["description"] = new OpenApiSchema { Type = "string" },
                ["receiptEmail"] = new OpenApiSchema { Type = "string", Format = "email" }
            },
            Required = new HashSet<string> { "amount", "currency", "description", "receiptEmail" }
        };

        var exampleObj = new OpenApiObject
        {
            ["amount"] = new OpenApiLong(12345),
            ["currency"] = new OpenApiString("usd"),
            ["description"] = new OpenApiString("Ecommerce purchase - Product A"),
            ["receiptEmail"] = new OpenApiString("example@gmail.com")
        };

        operation.RequestBody.Content["application/json"] = new OpenApiMediaType
        {
            Schema = schema,
            Example = exampleObj
        };
    }
}
