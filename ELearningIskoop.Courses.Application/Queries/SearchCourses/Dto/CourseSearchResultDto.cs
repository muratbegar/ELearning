using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Queries.SearchCourses.Dto
{
    public record CourseSearchResultDto
    {
        public int ObjectId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string InstructorName { get; init; } = string.Empty;
        public string FormattedPrice { get; init; } = string.Empty;
        public bool IsFree { get; init; }
        public string Level { get; init; } = string.Empty;
        public string TotalDuration { get; init; } = string.Empty;
        public int CurrentStudentCount { get; init; }
        public string? ThumbnailUrl { get; init; }
        public List<string> CategoryNames { get; init; } = new();
        public double RelevanceScore { get; init; }
    }
}
