using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ELeraningIskoop.ServiceDefaults.HealthCheck
{
    // ELearning platformu için özel health check'lar
    public static class HealthCheckExtensions
    {
        // Database health check'ı ekler
        public static IServiceCollection AddDatabaseHealthCheck<TDbContext>(
            this IServiceCollection services,
            string name = "database",
            string[] tags = null!)
            where TDbContext : DbContext
        {
            services.AddHealthChecks()
                .AddDbContextCheck<TDbContext>(
                    name: name,
                    tags: tags ?? new[] { "ready", "database" });

            return services;
        }

        // Redis health check'ı ekler
        public static IServiceCollection AddRedisHealthCheck(
            this IServiceCollection services,
            string connectionString,
            string name = "redis",
            string[] tags = null!)
        {
            services.AddHealthChecks()
                .AddRedis(
                    redisConnectionString: connectionString,
                    name: name,
                    tags: tags ?? new[] { "ready", "cache" });

            return services;
        }

        // RabbitMQ health check'ı ekler
        public static IServiceCollection AddRabbitMQHealthCheck(
            this IServiceCollection services,
            string connectionString,
            string name = "rabbitmq",
            string[] tags = null!)
        {
            services.AddHealthChecks()
                .AddRabbitMQ(
                    rabbitConnectionString: connectionString,
                    name: name,
                    tags: tags ?? new[] { "ready", "messaging" });

            return services;
        }

        // Custom ELearning health check'ları
        public static IServiceCollection AddELearningHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                // Storage health check (dosya sistemi)
                .AddCheck<StorageHealthCheck>("storage", tags: new[] { "ready", "storage" })

                // External API health check (varsa)
                .AddCheck<ExternalApiHealthCheck>("external-api", tags: new[] { "ready", "external" });

            return services;
        }
    }
}
