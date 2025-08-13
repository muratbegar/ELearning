using ELearningIskoop.Courses.Application.Queries.GetCourse;
using ELearningIskoop.Courses.Application.Queries.GetCourse.Dto;
using ELearningIskoop.Courses.Application.Queries.SearchCourses;
using ELearningIskoop.Courses.Application.Queries.SearchCourses.Dto;
using ELearningIskoop.Courses.Application.Services;
using ELearningIskoop.Shared.Domain.Enums;

namespace ELearningIskoop.API.GraphQL
{
    // GraphQL Query definitions
    public class CourseQueries
    {
        // Get a course by ID
        [GraphQLDescription("Retrieves a course by its unique identifier")]
        public async Task<GetCourseResponse> GetCourseAsync(
            [GraphQLDescription("The unique identifier of the course")] int id,
            [Service] ICourseApplicationService courseService,
            CancellationToken cancellationToken)
        {
            return await courseService.GetCourseByIdAsync(id, cancellationToken);
        }


        // Get courses with filtering and pagination
        [GraphQLDescription("Retrieves courses with optional filtering and pagination")]
        [UsePaging(IncludeTotalCount = true)]
        [UseFiltering]
        [UseSorting]
        public async Task<IQueryable<CourseListItemDto>> GetCoursesAsync(
            [Service] ICourseApplicationService courseService,
            CancellationToken cancellationToken,
            int first = 20,
            string? after = null,
            int? categoryId = null,
            bool onlyPublished = true)
        {
            var query = new GetCoursesQuery
            {
                PageSize = first,
                CategoryId = categoryId,
                OnlyPublished = onlyPublished
            };

            var result = await courseService.GetCoursesAsync(query, cancellationToken);

            var mapped = result.SelectMany(x => x.Courses).AsQueryable();//selectmany düz liste

            // Convert to IQueryable for GraphQL processing
            return mapped;
        }

       
        // Search courses

        [GraphQLDescription("Search courses by title, description, or instructor")]
        public async Task<SearchCoursesResponse> SearchCoursesAsync(
            [GraphQLDescription("Search term")] string searchTerm,
            [Service] ICourseApplicationService courseService,
            CancellationToken cancellationToken,
            int first = 20,
            int? categoryId = null,
            CourseLevel? level = null,
            bool? isFree = null)
        {
            var query = new SearchCoursesQuery
            {
                SearchTerm = searchTerm,
                PageSize = first,
                CategoryId = categoryId.Value,
                Level = level,
                IsFree = isFree
            };

            return await courseService.SearchCoursesAsync(query, cancellationToken);
        }
    }
}
