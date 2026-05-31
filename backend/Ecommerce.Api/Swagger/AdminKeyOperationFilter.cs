using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Ecommerce.Api.Attributes;
using System.Reflection;

namespace Ecommerce.Api.Swagger
{
    public class AdminKeyOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check if the controller or action has AdminKeyAttribute
            var hasAdminKeyAttribute = context.MethodInfo.DeclaringType?.GetCustomAttribute<AdminKeyAttribute>() != null ||
                                     context.MethodInfo.GetCustomAttribute<AdminKeyAttribute>() != null;

            if (hasAdminKeyAttribute)
            {
                operation.Parameters ??= new List<OpenApiParameter>();
                
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Admin-Key",
                    In = ParameterLocation.Header,
                    Required = true,
                    Description = "Admin access key required for this endpoint",
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Default = new Microsoft.OpenApi.Any.OpenApiString("123-admin-key")
                    }
                });

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "AdminKey"
                                }
                            },
                            new string[] {}
                        }
                    }
                };
            }
        }
    }
}