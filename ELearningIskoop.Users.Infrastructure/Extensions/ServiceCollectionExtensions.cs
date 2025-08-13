using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Domain.Repositories;
using ELearningIskoop.Users.Infrastructure.Persistence;
using ELearningIskoop.Users.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<UserDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("UserConnection");
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(UserDbContext).Assembly.FullName);
                    builder.EnableRetryOnFailure(maxRetryCount: 3);
                });

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static async Task ApplyUserMigrationsAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();

            await context.Database.MigrateAsync();
        }
    }
}
