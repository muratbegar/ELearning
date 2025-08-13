using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Queries.GetCourse.Dto
{
    public record CourseListItemDto
    {
        public int ObjectId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string InstructorName { get; init; } = string.Empty;
        public string FormattedPrice { get; init; } = string.Empty;
        public bool IsFree { get; init; }
        public string Level { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string TotalDuration { get; init; } = string.Empty;
        public int CurrentStudentCount { get; init; }
        public int MaxStudents { get; init; }
        public bool IsFull { get; init; }
        public string? ThumbnailUrl { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? PublishedAt { get; init; }
        public int LessonCount { get; init; }
        public List<string> CategoryNames { get; init; } = new();
    }
}
