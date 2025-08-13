using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ELearningIskoop.API.Swagger
{
    public class SwaggerTagsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new List<OpenApiTag>
            {
                new() { Name = "Courses",Description = "Course management operations"},
                new() { Name = "Lessons", Description = "Lesson management operations" },
                new() { Name = "Categories", Description = "Category management operations" },
                new() { Name = "Search", Description = "Search and filtering operations" },
                new() { Name = "Authentication", Description = "User authentication operations" },
                new() { Name = "GraphQL", Description = "GraphQL endpoint for flexible queries" }
            };
        }
    }
}
