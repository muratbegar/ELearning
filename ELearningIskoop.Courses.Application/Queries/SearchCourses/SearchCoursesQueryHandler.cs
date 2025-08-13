using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Courses.Application.Queries.SearchCourses.Dto;
using ELearningIskoop.Courses.Domain.Repositories;
using ELearningIskoop.Shared.Domain.Enums;

namespace ELearningIskoop.Courses.Application.Queries.SearchCourses
{
    public class SearchCoursesQueryHandler : IQueryHandler<SearchCoursesQuery, SearchCoursesResponse>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;
        public SearchCoursesQueryHandler(ICourseRepository courseRepository ,ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _courseRepository = courseRepository;
        }

        public async Task<SearchCoursesResponse> Handle(SearchCoursesQuery request, CancellationToken cancellationToken)
        {
            // Arama yap
            var courses = await _courseRepository.SearchAsync(
                searchTerm: request.SearchTerm,
                level: request.Level,
                categoryId: request.CategoryId,
                isFree: request.IsFree,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken
                );

            // Toplam sayıyı al (ayrı bir sorgu ile)
            var totalCount = await GetSearchResultCountAsync(request, cancellationToken);

            // DTO'lara map et
            var courseDtos = courses.Select(course => new CourseSearchResultDto
            {
                ObjectId = course.ObjectId,
                Title = course.Title,
                Description = TruncateDescription(course.Description, 150),
                InstructorName = course.InstructorName.FullName,
                FormattedPrice = course.Price.GetFormattedAmount(),
                IsFree = course.IsFree,
                Level = course.Level.GetDescription(),
                TotalDuration = course.TotalDuration.GetFormattedDuration(),
                CurrentStudentCount = course.CurrentStudentCount,
                ThumbnailUrl = course.ThumbnailUrl,
                CategoryNames = course.Categories.Select(cc => cc.Category.Name).ToList(),
                RelevanceScore = CalculateRelevanceScore(course, request.SearchTerm)
            }).ToList();

            // Facet'ları oluştur
            var facets = await BuildSearchFacetsAsync(request.SearchTerm, cancellationToken);

            return new SearchCoursesResponse
            {
                Courses = courseDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SearchTerm = request.SearchTerm,
                Facets = facets
            };
        }
        private async Task<List<SearchFacetDto>> BuildSearchFacetsAsync(string searchTerm, CancellationToken cancellationToken)
        {
            var facets = new List<SearchFacetDto>();

            // Kategori facet'ı
            var categoriesWithCount = await _categoryRepository.GetWithCourseCountAsync(cancellationToken);
            var categoryFacet = new SearchFacetDto
            {
                Name = "category",
                DisplayName = "Kategori",
                Values = categoriesWithCount
                    .Where(c => c.CourseCount > 0)
                    .Select(c => new SearchFacetValueDto
                    {
                        Value = c.CategoryId.ToString(),
                        DisplayName = c.CategoryName,
                        Count = c.CourseCount
                    })
                    .OrderByDescending(v => v.Count)
                    .Take(10)
                    .ToList()
            };
            facets.Add(categoryFacet);

            // Seviye facet'ı
            var levelFacet = new SearchFacetDto
            {
                Name = "level",
                DisplayName = "Seviye",
                Values = Enum.GetValues<CourseLevel>()
                    .Select(level => new SearchFacetValueDto
                    {
                        Value = ((int)level).ToString(),
                        DisplayName = level.GetDescription(),
                        Count = 0 // Bu gerçek implementasyonda hesaplanmalı
                    })
                    .ToList()
            };
            facets.Add(levelFacet);

            // Fiyat facet'ı
            var priceFacet = new SearchFacetDto
            {
                Name = "price",
                DisplayName = "Fiyat",
                Values = new List<SearchFacetValueDto>
            {
                new() { Value = "free", DisplayName = "Ücretsiz", Count = 0 },
                new() { Value = "0-100", DisplayName = "0-100 TL", Count = 0 },
                new() { Value = "100-500", DisplayName = "100-500 TL", Count = 0 },
                new() { Value = "500+", DisplayName = "500+ TL", Count = 0 }
            }
            };
            facets.Add(priceFacet);

            return facets;
        }
        private async Task<int> GetSearchResultCountAsync(SearchCoursesQuery request, CancellationToken cancellationToken)
        {
            var allResults = await _courseRepository.SearchAsync(
                searchTerm: request.SearchTerm,
                level: request.Level,
                categoryId: request.CategoryId,
                isFree: request.IsFree,
                pageNumber: 1,
                pageSize: int.MaxValue,
                cancellationToken: cancellationToken);

            return allResults.Count();
        }
        private static double CalculateRelevanceScore(Domain.Entities.Course course, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return 1.0;

            var score = 0.0;
            var normalizedSearchTerm = searchTerm.ToLowerInvariant();

            // Title match (en yüksek puan)
            if (course.Title.ToLowerInvariant().Contains(normalizedSearchTerm))
            {
                score += 10.0;
                if (course.Title.ToLowerInvariant().StartsWith(normalizedSearchTerm))
                    score += 5.0;
            }

            // Description match
            if (course.Description.ToLowerInvariant().Contains(normalizedSearchTerm))
                score += 3.0;

            // Instructor name match
            if (course.InstructorName.FullName.ToLowerInvariant().Contains(normalizedSearchTerm))
                score += 2.0;

            // Category match
            foreach (var category in course.Categories)
            {
                if (category.Category.Name.ToLowerInvariant().Contains(normalizedSearchTerm))
                    score += 1.0;
            }

            // Popularity bonus (öğrenci sayısı)
            score += Math.Log(course.CurrentStudentCount + 1) * 0.1;

            return Math.Max(score, 0.1); // Minimum score
        }
        private static string TruncateDescription(string description, int maxLength)
        {
            if (string.IsNullOrEmpty(description) || description.Length <= maxLength)
                return description;

            return description.Substring(0, maxLength) + "...";
        }


    }
}
