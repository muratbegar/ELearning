using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Queries.SearchCourses.Dto
{
    // Arama facet'ı (filtre seçenekleri)
    public record SearchFacetDto
    {
        public string Name { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public List<SearchFacetValueDto> Values { get; init; } = new();
    }
}
