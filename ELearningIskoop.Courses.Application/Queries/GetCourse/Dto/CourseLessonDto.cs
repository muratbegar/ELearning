using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Queries.GetCourse.Dto
{
    public record CourseLessonDto
    {
        public int ObjectId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Duration { get; init; } = string.Empty;
        public string ContentType { get; init; } = string.Empty;
        public int Order { get; init; }
        public string? ContentUrl { get; init; }
        public bool IsFree { get; init; } = false;
        public bool IsPublished { get; init; } = false;
    }
}
