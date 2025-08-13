using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Courses.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Application.Queries.GetCourse.Dto;
using ELearningIskoop.Courses.Domain.Enums;
using ELearningIskoop.Shared.Domain.Enums;

namespace ELearningIskoop.Courses.Application.Queries.GetCourse
{
    public class GetCourseQueryHandler : IQueryHandler<GetCourseQuery, GetCourseResponse>
    {
        private readonly ICourseRepository _courseRepository;

        public GetCourseQueryHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<GetCourseResponse> Handle(
            GetCourseQuery request,
            CancellationToken cancellationToken)
        {
            // Kursu getir
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
            {
                throw new EntityNotFoundException("Course", request.CourseId);
            }

            // DTO'ya map et
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
                Lessons = course.Lessons.OrderBy(l => l.Order).Select(l => new CourseLessonDto
                {
                    ObjectId = l.ObjectId,
                    Title = l.Title,
                    Description = l.Description,
                    Duration = l.Duration.GetFormattedDuration(),
                    ContentType = l.ContentType.GetDescription(),
                    Order = l.Order,
                    IsPublished = l.IsPublished,
                    IsFree = l.IsFree,
                    ContentUrl = l.IsFree ? l.GetContentUrl() : null // Sadece ücretsiz dersler için URL ver
                }).ToList(),
                Categories = course.Categories.Select(cc => new CourseCategoryDto
                {
                    ObjectId = cc.Category.ObjectId,
                    Name = cc.Category.Name,
                    Slug = cc.Category.Slug,
                    Color = cc.Category.Color,
                    IconUrl = cc.Category.IconUrl
                }).ToList()
            };
        }
    }
}
