using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.Behaviors;
using ELearningIskoop.Courses.Application.Queries.GetCourse.Dto;
using ELearningIskoop.BuildingBlocks.Application.CQRS;

namespace ELearningIskoop.Courses.Application.Queries.GetCourse
{
    // Kurs detay getirme sorgusu
    public record GetCourseQuery : BaseQuery<GetCourseResponse>, ICacheableRequest
    {
       
        // Kurs ID'si
        public int CourseId { get; init; }

  
        // Cache key'i
        public string CacheKey => $"course:{CourseId}";

 
        // Cache süresi - 15 dakika
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(15);
    }

    // Kurs detay response'u
    public record GetCourseResponse
    {
        public int ObjectId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string InstructorName { get; init; } = string.Empty;
        public string InstructorEmail { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Currency { get; init; } = string.Empty;
        public string FormattedPrice { get; init; } = string.Empty;
        public bool IsFree { get; init; }
        public string Level { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime? PublishedAt { get; init; }
        public string TotalDuration { get; init; } = string.Empty;
        public int MaxStudents { get; init; }
        public int CurrentStudentCount { get; init; }
        public bool IsFull { get; init; }
        public bool IsAvailable { get; init; }
        public string? ThumbnailUrl { get; init; }
        public string? TrailerVideoUrl { get; init; }
        public DateTime CreatedAt { get; init; }
        public List<CourseLessonDto> Lessons { get; init; } = new();
        public List<CourseCategoryDto> Categories { get; init; } = new();
    }
}
