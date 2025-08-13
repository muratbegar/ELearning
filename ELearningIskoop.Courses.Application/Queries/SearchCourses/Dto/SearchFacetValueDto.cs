using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Queries.SearchCourses.Dto
{
    // Arama facet değeri
    public record SearchFacetValueDto
    {
        public string Value { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public int Count { get; init; }
    }
}
