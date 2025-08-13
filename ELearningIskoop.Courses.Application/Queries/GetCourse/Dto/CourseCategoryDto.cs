using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Queries.GetCourse.Dto
{
    public record CourseCategoryDto
    {
        public int ObjectId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public string? Color { get; init; }
        public string? IconUrl { get; init; }
    }
}
