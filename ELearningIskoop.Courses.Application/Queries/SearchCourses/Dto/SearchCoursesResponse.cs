using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Queries.SearchCourses.Dto
{
    public record SearchCoursesResponse
    {
        public List<CourseSearchResultDto> Courses { get; init; } = new();
        public int TotalCount { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public string SearchTerm { get; init; } = string.Empty;
        public List<SearchFacetDto> Facets { get; init; } = new();
    }
}
