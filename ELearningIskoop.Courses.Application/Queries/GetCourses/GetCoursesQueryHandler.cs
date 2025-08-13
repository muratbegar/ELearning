using ELearningIskoop.Courses.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Application.Queries.GetCourse;
using ELearningIskoop.Courses.Application.Queries.GetCourse.Dto;
using ELearningIskoop.Courses.Application.Queries.GetCourse.Enums;
using ELearningIskoop.Courses.Domain.Enums;
using ELearningIskoop.Shared.Domain.Enums;

namespace ELearningIskoop.Courses.Application.Queries.GetCourses
{
    // Kurs listesi getirme sorgusu handler'ı
    public class GetCoursesQueryHandler
    {
        private readonly ICourseRepository _courseRepository;

        public GetCoursesQueryHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<GetCoursesResponse> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            //Filtreleme kriterleri
            var courses = await GetFilteredCoursesAsync(request, cancellationToken);

            //toplam saniye hesapla
            var totalCount = courses.Count();

            //sayfalamayı uygula
            var pagedCourses = courses
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // DTO'lara map et
            var courseDtos = pagedCourses.Select(course => new CourseListItemDto
            {
                ObjectId = course.ObjectId,
                Title = course.Title,
                Description = TruncateDescription(course.Description, 200),
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
            }).ToList();

            // Sayfalama bilgilerini hesapla
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            return new GetCoursesResponse
            {
                Courses = courseDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.PageNumber < totalPages,
                HasPreviousPage = request.PageNumber > 1
            };
        }

        private async Task<IEnumerable<Domain.Entities.Course>> GetFilteredCoursesAsync(GetCoursesQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Course> courses;

            //Temel filtreleme
            if (!string.IsNullOrEmpty(request.InstructorEmail))
            {
                courses = await _courseRepository.GetByInstructorEmailAsync(request.InstructorEmail, cancellationToken);
            }
            else if(request.CategoryId.HasValue)
            {
                courses = await _courseRepository.GetByCategoryAsync(request.CategoryId.Value, cancellationToken);
            }
            else if (request.OnlyPublished)
            {
                courses = await _courseRepository.GetPublishedCoursesAsync(request.PageNumber, request.PageSize, cancellationToken);
            }
            else
            {
                courses = await _courseRepository.GetAllAsync(cancellationToken);
            }

            //sıralama
            courses = ApplySorting(courses, request.SortBy, request.SortDirection);

            return courses;
        }

        private static IEnumerable<Domain.Entities.Course> ApplySorting(IEnumerable<Domain.Entities.Course> courses,
            CourseSortBy sortBy, SortDirection sortDirection)
        {
            var orderedCourses = sortBy switch
            {
                CourseSortBy.Title => sortDirection == SortDirection.Ascending
                    ? courses.OrderBy(x => x.Title)
                    : courses.OrderByDescending(x => x.Title),
                CourseSortBy.Price => sortDirection == SortDirection.Ascending
                    ? courses.OrderBy(x => x.Price.Amount)
                    : courses.OrderByDescending(x => x.Price.Amount),
                CourseSortBy.StudentCount => sortDirection == SortDirection.Ascending
                    ? courses.OrderBy(x => x.CurrentStudentCount)
                    : courses.OrderByDescending(x => x.CurrentStudentCount),
                CourseSortBy.PublishedAt => sortDirection == SortDirection.Ascending
                    ? courses.OrderBy(x => x.PublishedAt)
                    : courses.OrderByDescending(x => x.PublishedAt),
                _ => sortDirection == SortDirection.Ascending
                    ? courses.OrderBy(x => x.CreatedAt)
                    : courses.OrderByDescending(x => x.CreatedAt)
            };

            return orderedCourses;
        }


        private static string TruncateDescription(string description, int maxLength)
        {
            if (string.IsNullOrEmpty(description) || description.Length <= maxLength)
            {
                return description;
            }
            return description.Substring(0, maxLength) + "...";
        }
    }
}
