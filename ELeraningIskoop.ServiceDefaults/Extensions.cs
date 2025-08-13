using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ServiceDiscovery;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;

namespace Microsoft.Extensions.Hosting
{
    // Tüm Aspire servisleri için ortak konfigürasyonlar
    public static class Extensions
    {
        // Tüm service defaults'larý ekler
        public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
        {
            builder.ConfigureSerilog();
            builder.ConfigureOpenTelemetry();
            builder.AddDefaultHealthChecks();
            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                // Resilience patterns (retry, circuit breaker, timeout)
                http.AddStandardResilienceHandler();

                // Service discovery
                http.AddServiceDiscovery();
            });

            return builder;
        }

        // Serilog konfigürasyonu
        public static IHostApplicationBuilder ConfigureSerilog(this IHostApplicationBuilder builder)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .ConfigureBaseLogging(builder.Environment, builder.Configuration);

            // Development ortamýnda daha detaylý log
            if (builder.Environment.IsDevelopment())
            {
                loggerConfiguration
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
            }
            else
            {
                loggerConfiguration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning);
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            builder.Services.AddSerilog(Log.Logger);

            return builder;
        }


        // Serilog temel konfigürasyonu
        private static LoggerConfiguration ConfigureBaseLogging(
        this LoggerConfiguration config,
        IHostEnvironment environment,
        IConfiguration configuration)
        {
            // Ortak enricher'lar
            config
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "ELearning.Platform")
                .Enrich.WithProperty("Environment", environment.EnvironmentName)
                .Enrich.WithMachineName()
                .Enrich.WithThreadId();

            // Console sink - development'ta güzel format, production'da JSON
            if (environment.IsDevelopment())
            {
                config.WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code);
            }
            else
            {
                config.WriteTo.Console(new CompactJsonFormatter());
            }

            // File sink - rolling dosyalar
            config.WriteTo.File(
                path: "logs/elearning-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                formatter: new CompactJsonFormatter(),
                fileSizeLimitBytes: 100 * 1024 * 1024); // 100MB

            // Error dosyasý - sadece error ve fatal
            config.WriteTo.File(
                path: "logs/errors/elearning-error-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 90,
                restrictedToMinimumLevel: LogEventLevel.Error,
                formatter: new CompactJsonFormatter());

            // Seq sink - development ve staging için
            var seqUrl = configuration.GetConnectionString("Seq");
            if (!string.IsNullOrEmpty(seqUrl))
            {
                config.WriteTo.Seq(seqUrl);
            }

            // Application Insights (Azure için)
            var appInsightsKey = configuration["ApplicationInsights:InstrumentationKey"];
            if (!string.IsNullOrEmpty(appInsightsKey))
            {
                config.WriteTo.ApplicationInsights(appInsightsKey, TelemetryConverter.Traces);
            }

            return config;
        }

        // OpenTelemetry konfigürasyonu
        public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                {
                    resource.AddService(
                        serviceName: "ELearning.Platform",
                        serviceVersion: "1.0.0",
                        serviceInstanceId: Environment.MachineName);
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        //.AddProcessInstrumentation() //OpenTelemetry.Instrumentation.Process), or the method simply does not exist in the version of OpenTelemetry you are using.
                        .AddMeter("ELearning.*"); // Custom meter'larýmýz için
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            // Sensitive data'yý trace etme
                            options.Filter = context => !context.Request.Path.StartsWithSegments("/health");
                            options.EnrichWithHttpRequest = (activity, request) =>
                            {
                                activity.SetTag("user.id", request.Headers["X-User-Id"].FirstOrDefault());
                            };
                        })
                        .AddHttpClientInstrumentation()
                        //.AddEntityFrameworkCoreInstrumentation(options =>
                        //{
                        //    options.SetDbStatementForText = true;
                        //    options.SetDbStatementForStoredProcedure = true;
                        //})
                        .AddSource("ELearning.*") // Custom activity source'larýmýz için
                        .AddSource("MediatR"); // MediatR tracing için
                });

            builder.AddOpenTelemetryExporters();

            return builder;
        }

        // OpenTelemetry exporter'larý
        private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
        {
            //var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

            //if (useOtlpExporter)
            //{
            //    builder.Services.AddOpenTelemetry().UseOtlpExporter();
            //}

            // Jaeger (development için)
            if (builder.Environment.IsDevelopment())
            {
                var jaegerEndpoint = builder.Configuration.GetConnectionString("Jaeger");
                if (!string.IsNullOrEmpty(jaegerEndpoint))
                {
                    builder.Services.AddOpenTelemetry()
                        .WithTracing(tracing => tracing.AddJaegerExporter());
                }
            }

            return builder;
        }

        // Default health check'larý ekler
        public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                // Temel uygulama saðlýðý
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])

                // Memory kullanýmý
                .AddProcessAllocatedMemoryHealthCheck(
                    maximumMegabytesAllocated: 1024, // 1GB
                    name: "memory",
                    tags: ["memory"])

                // Disk alaný
                .AddDiskStorageHealthCheck(options =>
                {
                    options.AddDrive("C:", minimumFreeMegabytes: 1024); // 1GB minimum
                }, name: "disk", tags: ["disk"]);

            return builder;
        }

        // Default endpoint'leri map eder
        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            // Health check endpoint'leri
            app.MapHealthChecks("/health");
            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live")
            });

            // OpenTelemetry endpoint'leri (sadece development)
            if (app.Environment.IsDevelopment())
            {
                // Prometheus metrics endpoint
                //app.MapPrometheusScrapingEndpoint();
            }

            return app;
        }
    }
}
