using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace ELearningIskoop.API.RateLimiting
{
    public static class RateLimitingExtensions
    {
        // Rate limiting policies'ini konfigüre eder
        public static IServiceCollection AddRateLimitingConfiguration(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Global rate limit
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100, // 100 request
                            Window = TimeSpan.FromMinutes(1) // 1 dakikada
                        }));


                // API specific policies

                // 1. Authentication endpoints - daha sıkı limit
                options.AddPolicy("AuthPolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 5, // 5 request
                            Window = TimeSpan.FromMinutes(1) // 1 dakikada
                        }));

                // 2. Search endpoints - orta seviye limit
                options.AddPolicy("SearchPolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 30, // 30 request
                            Window = TimeSpan.FromMinutes(1) // 1 dakikada
                        }));

                // 3. Premium users - yüksek limit
                options.AddPolicy("PremiumPolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? "anonymous",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 200, // 200 request
                            Window = TimeSpan.FromMinutes(1) // 1 dakikada
                        }));

                // 4. File upload - en düşük limit
                options.AddPolicy("UploadPolicy", httpContext =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        factory: partition => new SlidingWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 3, // 3 upload
                            Window = TimeSpan.FromMinutes(10), // 10 dakikada
                            SegmentsPerWindow = 2
                        }));


                // Rate limit exceeded response
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    context.HttpContext.Response.ContentType = "application/json";

                    var response = new
                    {
                        error = "Rate limit exceeded",
                        message = "Too many requests. Please try again later.",
                        retryAfter = GetRetryAfterSeconds(context)
                    };

                    await context.HttpContext.Response.WriteAsync(
                        System.Text.Json.JsonSerializer.Serialize(response), token);
                };
            });
            return services;
        }

        private static int GetRetryAfterSeconds(OnRejectedContext context)
        {
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                return (int)retryAfter.TotalSeconds;
            }
            return 60; // Default 1 dakika
        }
    }
}
