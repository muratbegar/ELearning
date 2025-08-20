using IskoopDemo.Shared.Application.Interfaces;
using IskoopDemo.Shared.Domain.Entities;
using IskoopDemo.Shared.Domain.Events;
using IskoopDemo.Shared.Infrastructure.Caching;
using IskoopDemo.Shared.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using IskoopDemo.Shared.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Interfaces;

namespace IskoopDemo.Shared.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // Adds shared infrastructure services to the DI container
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            //add basic services
            services.AddDateTimeProvider();
            services.AddCurrentUserService();
            services.AddEventDispatcher();


            // Add caching
           // services.AddCachingServices(configuration);
            // Add repository pattern
            services.AddRepositoryPattern();

            return services;

        }

        public static IServiceCollection AddDateTimeProvider(this IServiceCollection services)
        {
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            return services;
        }

        public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, ICurrentUserService>();
            return services;
        }

        // Adds event dispatcher service
        public static IServiceCollection AddEventDispatcher(this IServiceCollection services)
        {
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            return services;
        }

        //// Adds caching services based on configuration
        //public static IServiceCollection AddCachingServices(this IServiceCollection services,
        //    IConfiguration configuration)
        //{
        //    var cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>();

        //    switch (cacheSettings?.Provider?.ToLowerInvariant())
        //    {
        //        case "redis":
        //            services.AddRedisCaching(configuration);
        //            break;
        //        case "hybrid":
        //            services.AddHybridCaching(configuration);
        //            break;
        //        case "memory":
        //        default:
        //            services.AddMemoryCaching();
        //            break;
        //    }

        //    return services;
        //}

        // Adds memory caching only
        public static IServiceCollection AddMemoryCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            return services;

        }

        // Adds Redis distributed caching
        public static IServiceCollection AddRedisCaching(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Redis");
                    
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = "ELearning";
            });

            services.AddSingleton<ICacheService, DistributedCacheService>();
            return services;
        }

        /// <summary>
        /// Adds hybrid caching (Memory + Redis)
        /// </summary>
        public static IServiceCollection AddHybridCaching(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add both memory and distributed cache
            services.AddMemoryCache();

            var connectionString = configuration.GetConnectionString("Redis");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = "ELearning";
            });

            services.AddSingleton<ICacheService, HybridCacheService>();
            return services;
        }

        /// <summary>
        /// Adds repository pattern services
        /// </summary>
        public static IServiceCollection AddRepositoryPattern(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        /// <summary>
        /// Adds Entity Framework with specific DbContext
        /// </summary>
        public static IServiceCollection AddEntityFramework<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionStringName = "DefaultConnection")
            where TContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionStringName);

            services.AddDbContext<TContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    sqlOptions.CommandTimeout(30);
                });

                // Development settings
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            // Register as IUnitOfWork if the context implements it
            if (typeof(IUnitOfWork).IsAssignableFrom(typeof(TContext)))
            {
                services.AddScoped<IUnitOfWork>(provider =>
                    (IUnitOfWork)provider.GetRequiredService<TContext>());
            }

            return services;
        }

       
    }
}
