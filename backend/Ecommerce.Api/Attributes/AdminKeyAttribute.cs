using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminKeyAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var expectedKey = config["Admin:AccessKey"];
            var providedKey = context.HttpContext.Request.Headers["X-Admin-Key"].FirstOrDefault();

            if (string.IsNullOrEmpty(expectedKey) || providedKey != expectedKey)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid or missing admin key." });
            }
        }
    }
}
