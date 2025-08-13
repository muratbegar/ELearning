using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELeraningIskoop.ServiceDefaults.HealthCheck
{
    public class ExternalApiHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ExternalApiHealthCheck(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Örnek: Payment gateway health check
                using var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(10);

                // Bu endpoint gerçek bir external service olacak
                var response = await client.GetAsync("https://httpbin.org/status/200", cancellationToken);

                return response.IsSuccessStatusCode
                    ? HealthCheckResult.Healthy("External API is accessible")
                    : HealthCheckResult.Unhealthy($"External API returned {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("External API check failed", ex);
            }
        }
    }
}
