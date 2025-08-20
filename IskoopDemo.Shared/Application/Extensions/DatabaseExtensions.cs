using IskoopDemo.Shared.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Extensions
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Configures common database settings
        /// </summary>
        public static void ConfigureCommonDatabase(this DbContextOptionsBuilder options, string connectionString)
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                // Connection resiliency
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                // Performance settings
                sqlOptions.CommandTimeout(30);
                sqlOptions.MigrationsAssembly("ELearning.Infrastructure");
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
            });

            // Performance optimizations
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            // Development settings
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }
        }

        /// <summary>
        /// Applies pending migrations and seeds data
        /// </summary>
        public static async Task InitializeDatabaseAsync<TContext>(this IServiceProvider serviceProvider)
            where TContext : DbContext
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                // Apply migrations
                await context.Database.MigrateAsync();

                // Seed data if method exists
                if (context is ISeeder seeder)
                {
                    await seeder.SeedAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetService<ILogger<TContext>>();
                logger?.LogError(ex, "An error occurred while migrating or seeding the database");
                throw;
            }
        }
    }
    //     }
    // 
    //     // Additional health checks can be added here
    // 
    //     return services;
    // }
}
