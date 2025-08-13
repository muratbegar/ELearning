using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ELearningIskoop.API.Swagger
{
    public class SwaggerExamplesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //Rate Limiting Headers

            if (operation.Responses.ContainsKey("200"))
            {
                operation.Responses["200"].Headers ??= new Dictionary<string, OpenApiHeader>();
                operation.Responses["200"].Headers["X-RateLimit-Limit"] = new OpenApiHeader
                {
                    Description = "The maximum number of requests allowed per time window",
                    Schema = new OpenApiSchema { Type = "integer" }
                };
                operation.Responses["200"].Headers["X-RateLimit-Remaining"] = new OpenApiHeader
                {
                    Description = "The number of requests remaining in the current time window",
                    Schema = new OpenApiSchema { Type = "integer" }
                };
            }


            // 429 Too Many Requests
            operation.Responses["429"] = new OpenApiResponse
            {
                Description = "Too Many Requests - Rate limit exceeded",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["error"] = new OpenApiSchema { Type = "string" },
                                ["message"] = new OpenApiSchema { Type = "string" },
                                ["retryAfter"] = new OpenApiSchema { Type = "integer" }
                            }
                        }
                    }
                }
            };
        }
    }
}
