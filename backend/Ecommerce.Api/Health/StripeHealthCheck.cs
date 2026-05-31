using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ecommerce.Api.Health;

public class StripeHealthCheck : IHealthCheck
{
    private readonly string _apiKey;
    private readonly IHttpClientFactory _clients;

    public StripeHealthCheck(IConfiguration configuration, IHttpClientFactory clients)
    {
        _apiKey = configuration["Stripe:SecretKey"] ?? string.Empty;
        _clients = clients;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            return HealthCheckResult.Unhealthy("Stripe API key not configured");
        }

        try
        {
            var client = _clients.CreateClient("stripe-check");
            var req = new HttpRequestMessage(HttpMethod.Get, "https://api.stripe.com/v1/account");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var resp = await client.SendAsync(req, cancellationToken);
            if (resp.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Stripe reachable");
            }

            var body = await resp.Content.ReadAsStringAsync(cancellationToken);
            return HealthCheckResult.Unhealthy($"Stripe returned {(int)resp.StatusCode}: {body}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Stripe check failed: " + ex.Message);
        }
    }
}
