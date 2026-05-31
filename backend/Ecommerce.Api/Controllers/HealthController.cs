using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Linq;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet("healthz")]
        public async Task<IActionResult> Liveness()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            var status = report.Status;

            var result = new
            {
                status = status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    data = e.Value.Data
                })
            };

            return status == HealthStatus.Healthy ? Ok(result) : StatusCode(503, result);
        }

        [HttpGet("ready")]
        public async Task<IActionResult> Readiness()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            var status = report.Status;

            var result = new
            {
                status = status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    data = e.Value.Data
                })
            };

            return status == HealthStatus.Healthy ? Ok(result) : StatusCode(503, result);
        }
    }
}
