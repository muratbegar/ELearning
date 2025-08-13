using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace ELearningIskoop.API.Versioning
{
    public static class ApiVersioningExtensions
    {
        public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(DateTime.Now);
                options.AssumeDefaultVersionWhenUnspecified = true;

                //Versioning Strategies
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(), // /api/v1/courses
                    new HeaderApiVersionReader("X-Version"), // Header: X-Version: 1.0
                    new QueryStringApiVersionReader("version") // ?version=1.0
                );
                // Version format
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });

            // Fix: Use AddVersionedApiExplorer instead of AddApiExplorer
            services.AddVersionedApiExplorer(options =>
            {
                // Swagger için version format
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}
