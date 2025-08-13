using ELearningIskoop.BuildingBlocks.Application.Behaviors;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Application.Queries.GetCourse.Dto;
using ELearningIskoop.Courses.Application.Queries.GetCourse.Enums;

namespace ELearningIskoop.Courses.Application.Queries.GetCourse
{
    // Kurs listesi getirme sorgusu
    public record GetCoursesQuery : BaseQuery<List<GetCoursesResponse>>, ICacheableRequest
    {
        // Sayfa numarası (varsayılan: 1)
        public int PageNumber { get; init; } = 1;

        // Sayfa boyutu (varsayılan: 20)
        public int PageSize { get; init; } = 20;

        // Kategori ID'si (filtreleme için)
        public int? CategoryId { get; init; }

        // Kurs seviyesi (filtreleme için)
        public CourseLevel? Level { get; init; }

        // Sadece ücretsiz kurslar mı?
        public bool? IsFree { get; init; }

        // Sadece yayındaki kurslar mı?
        public bool OnlyPublished { get; init; } = true;

        // Eğitmen email'i (eğitmenin kurslarını getirmek için)
        public string? InstructorEmail { get; init; }

        // Sıralama (varsayılan: CreatedAt)
        public CourseSortBy SortBy { get; init; } = CourseSortBy.CreatedAt;

        // Sıralama yönü (varsayılan: Descending)
        public SortDirection SortDirection { get; init; } = SortDirection.Descending;

        // Cache key'i
        public string CacheKey => $"courses:page:{PageNumber}:size:{PageSize}:cat:{CategoryId}:level:{Level}:free:{IsFree}:pub:{OnlyPublished}:inst:{InstructorEmail}:sort:{SortBy}:{SortDirection}";

        // Cache süresi - 5 dakika
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
    }
}
