using ELearningIskoop.Courses.Application.Queries.GetCourse;
using ELearningIskoop.Courses.Application.Queries.GetCourse.Dto;
using ELearningIskoop.Courses.Domain.Entities;
using ELearningIskoop.Courses.Domain.Enums;
using ELearningIskoop.Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Mappings
{
    // Kurs mapping extension metodları
    public static class CourseMappingExtensions
    {
        // Course entity'sini GetCourseResponse'a map eder
        public static GetCourseResponse ToGetCourseResponse(this Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            return new GetCourseResponse
            {
                ObjectId = course.ObjectId,
                Title = course.Title,
                Description = course.Description,
                InstructorName = course.InstructorName.FullName,
                InstructorEmail = course.InstructorEmail.Value,
                Price = course.Price.Amount,
                Currency = course.Price.Currency,
                FormattedPrice = course.Price.GetFormattedAmount(),
                IsFree = course.IsFree,
                Level = course.Level.GetDescription(),
                Status = course.Status.GetDescription(),
                PublishedAt = course.PublishedAt,
                TotalDuration = course.TotalDuration.GetFormattedDuration(),
                MaxStudents = course.MaxStudents,
                CurrentStudentCount = course.CurrentStudentCount,
                IsFull = course.IsFull(),
                IsAvailable = course.IsAvailable,
                ThumbnailUrl = course.ThumbnailUrl,
                TrailerVideoUrl = course.TrailerVideoUrl,
                CreatedAt = course.CreatedAt,
                Lessons = course.Lessons.OrderBy(l => l.Order).Select(l => l.ToCourseLessonDto()).ToList(),
                Categories = course.Categories.Select(cc => cc.Category.ToCourseCategoryDto()).ToList()
            };
        }

        // Course entity'sini CourseListItemDto'ya map eder
        public static CourseListItemDto ToCourseListItemDto(this Course course)
        {
            return new CourseListItemDto
            {
                ObjectId = course.ObjectId,
                Title = course.Title,
                Description = course.Description.Truncate(200),
                InstructorName = course.InstructorName.FullName,
                FormattedPrice = course.Price.GetFormattedAmount(),
                IsFree = course.IsFree,
                Level = course.Level.GetDescription(),
                Status = course.Status.GetDescription(),
                TotalDuration = course.TotalDuration.GetFormattedDuration(),
                CurrentStudentCount = course.CurrentStudentCount,
                MaxStudents = course.MaxStudents,
                IsFull = course.IsFull(),
                ThumbnailUrl = course.ThumbnailUrl,
                CreatedAt = course.CreatedAt,
                PublishedAt = course.PublishedAt,
                LessonCount = course.Lessons.Count,
                CategoryNames = course.Categories.Select(cc => cc.Category.Name).ToList()
            };
        }

        // Lesson entity'sini CourseLessonDto'ya map eder
        public static CourseLessonDto ToCourseLessonDto(this Lesson lesson)
        {
            return new CourseLessonDto
            {
                ObjectId = lesson.ObjectId,
                Title = lesson.Title,
                Description = lesson.Description,
                Duration = lesson.Duration.GetFormattedDuration(),
                ContentType = lesson.ContentType.GetDescription(),
                Order = lesson.Order,
                IsPublished = lesson.IsPublished,
                IsFree = lesson.IsFree,
                ContentUrl = lesson.IsFree ? lesson.GetContentUrl() : null
            };
        }

        // Category entity'sini CourseCategoryDto'ya map eder
        public static CourseCategoryDto ToCourseCategoryDto(this Category category)
        {
            return new CourseCategoryDto
            {
                ObjectId = category.ObjectId,
                Name = category.Name,
                Slug = category.Slug,
                Color = category.Color,
                IconUrl = category.IconUrl
            };
        }

        // String'i belirli uzunlukta keser
        private static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;

            return value.Substring(0, maxLength) + "...";
        }
    }
}
