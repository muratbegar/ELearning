using ELearningIskoop.Courses.Application.Commands.CreateCourse;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ELearningIskoop.API.Swagger
{
    // Swagger için örnek veri şema filtresi
    public class ExampleSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(CreateCourseCommand))
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["title"] = new Microsoft.OpenApi.Any.OpenApiString("C# ile Web Development"),
                    ["description"] =
                        new Microsoft.OpenApi.Any.OpenApiString("Sıfırdan ileri seviye C# web geliştirme kursu"),
                    ["instructorFirstName"] = new Microsoft.OpenApi.Any.OpenApiString("Ahmet"),
                    ["instructorLastName"] = new Microsoft.OpenApi.Any.OpenApiString("Yılmaz"),
                    ["instructorEmail"] = new Microsoft.OpenApi.Any.OpenApiString("ahmet@example.com"),
                    ["price"] = new Microsoft.OpenApi.Any.OpenApiDouble(299.99),
                    ["currency"] = new Microsoft.OpenApi.Any.OpenApiString("TRY"),
                    ["level"] = new Microsoft.OpenApi.Any.OpenApiInteger(2),
                    ["maxStudents"] = new Microsoft.OpenApi.Any.OpenApiInteger(1000),
                    ["categoryIds"] = new Microsoft.OpenApi.Any.OpenApiArray
                    {
                        new Microsoft.OpenApi.Any.OpenApiString("123e4567-e89b-12d3-a456-426614174000")
                    }
                };
            }
        }
    }
}
