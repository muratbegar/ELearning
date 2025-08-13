using Microsoft.OpenApi.Models;
using System.Reflection;

namespace ELearningIskoop.API.Swagger
{
    public static class SwaggerConfiguration
    {
        // Swagger'ı gelişmiş özelliklerle konfigüre eder
        public static IServiceCollection AddAdvancedSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // API Info
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ELearning Courses API",
                    Description = "Comprehensive e-learning course management API with advanced features",
                    Contact = new OpenApiContact
                    {
                        Name = "ELearning Team",
                        Email = "api-support@elearning.com",
                        Url = new Uri("https://elearning.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    },
                    TermsOfService = new Uri("https://elearning.com/terms")
                });

                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "ELearning Courses API v2",
                    Description = "Enhanced version with GraphQL support and advanced filtering"
                });

                // JWT Authentication
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });


                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // API Key Authentication (alternatif)
                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Name = "X-API-Key",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "API Key needed to access the endpoints"
                });

                // XML Documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                // Custom schema filters
                options.SchemaFilter<ExampleSchemaFilter>();
                options.OperationFilter<SwaggerExamplesOperationFilter>();
                options.DocumentFilter<SwaggerTagsDocumentFilter>();

                // Enable annotations
                options.EnableAnnotations();

                // Enum as string
                options.SchemaGeneratorOptions.UseAllOfToExtendReferenceSchemas = true;
                options.SupportNonNullableReferenceTypes();

                // Custom operation sorting
                options.OrderActionsBy(apiDesc =>
                    $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}_{apiDesc.RelativePath}");
            });

            return services;
        }
    }
}
