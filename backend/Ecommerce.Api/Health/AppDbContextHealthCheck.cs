using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Api.Models;

namespace Ecommerce.Api.Health;

public class AppDbContextHealthCheck : IHealthCheck
{
    private readonly AppDbContext _db;

    public AppDbContextHealthCheck(AppDbContext db)
    {
        _db = db;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _db.Database.CanConnectAsync(cancellationToken);
            return canConnect ? HealthCheckResult.Healthy("Database reachable") : HealthCheckResult.Unhealthy("Database not reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database check failed: " + ex.Message);
        }
    }
}
