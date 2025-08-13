using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELeraningIskoop.ServiceDefaults.HealthCheck
{
    // Dosya depolama health check'ı
    public class StorageHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Upload klasörünün var olup olmadığını kontrol et
                var uploadPath = Path.Combine(Environment.CurrentDirectory, "uploads");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Test dosyası yazma/okuma
                var testFile = Path.Combine(uploadPath, "health-check.txt");
                await File.WriteAllTextAsync(testFile, "health-check", cancellationToken);

                var content = await File.ReadAllTextAsync(testFile, cancellationToken);

                if (content != "health-check")
                {
                    return HealthCheckResult.Unhealthy("Storage read/write failed");
                }

                File.Delete(testFile);

                return HealthCheckResult.Healthy("Storage is working properly");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Storage check failed", ex);
            }
        }
    }
}
