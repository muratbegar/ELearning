
using ELearningIskoop.Courses.Application.Commands.AddLesson;
using ELearningIskoop.Courses.Application.Commands.CreateCourse;
using ELearningIskoop.Courses.Application.Commands.PublishCourse;
using ELearningIskoop.Courses.Application.Queries.GetCourse;
using ELearningIskoop.Courses.Application.Queries.GetCourse.Dto;
using ELearningIskoop.Courses.Application.Queries.GetCourse.Enums;
using ELearningIskoop.Courses.Application.Queries.SearchCourses;
using ELearningIskoop.Courses.Application.Queries.SearchCourses.Dto;
using ELearningIskoop.Courses.Application.Services;
using ELearningIskoop.Shared.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/courses")]
    [Authorize]
    [EnableRateLimiting("GlobalPolicy")]
    [Produces("application/json")]
    [SwaggerTag("Courses", "Course management operations")]
    public class CoursesController : ControllerBase
    {
        private readonly ILogger<CoursesController> _logger;
        private readonly ICourseApplicationService _courseService;

        public CoursesController(ICourseApplicationService courseService,ILogger<CoursesController> logger)
        {
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all courses with filtering and pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20, max: 100)</param>
        /// <param name="categoryId">Filter by category ID</param>
        /// <param name="level">Filter by course level</param>
        /// <param name="isFree">Filter by free courses</param>
        /// <param name="onlyPublished">Show only published courses (default: true)</param>
        /// <param name="sortBy">Sort by field</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of courses</returns>

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get courses with filtering and pagination",
            Description = "Retrieves a paginated list of courses with optional filtering by category, level, price, and other criteria."
        )]
        [SwaggerResponse(200, "Success", typeof(GetCoursesResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(ErrorResponse))]
        [SwaggerResponse(429, "Rate Limit Exceeded", typeof(ErrorResponse))]
        public async Task<ActionResult<GetCoursesResponse>> GetCourses(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? categoryId = null,
            [FromQuery] CourseLevel? level = null,
            [FromQuery] bool? isFree = null,
            [FromQuery] bool onlyPublished = true,
            [FromQuery] CourseSortBy sortBy = CourseSortBy.CreatedAt,
            [FromQuery] SortDirection sortDirection = SortDirection.Descending,
            CancellationToken cancellationToken = default)
        {
            var query = new GetCoursesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                CategoryId = categoryId,
                Level = level,
                IsFree = isFree,
                OnlyPublished = onlyPublished,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var result = await _courseService.GetCoursesAsync(query, cancellationToken);

            //Rate linit headers
            Response.Headers.Add("X-RateLimit-Limit", "100");
            Response.Headers.Add("X-RateLimit-Remaining", "99");

            return Ok(result);
        }





        /// <summary>
        /// Get a specific course by ID
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Course details</returns>


        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get course by ID",
            Description = "Retrieves detailed information about a specific course including lessons and categories."
        )]
        [SwaggerResponse(200, "Success", typeof(GetCourseResponse))]
        [SwaggerResponse(404, "Course not found", typeof(ErrorResponse))]
        [SwaggerResponse(429, "Rate Limit Exceeded", typeof(ErrorResponse))]
        public async Task<ActionResult<GetCourseResponse>> GetCourse(
            int id,
            CancellationToken cancellationToken = default)
        {
            var result = await _courseService.GetCourseByIdAsync(id, cancellationToken);
            return Ok(result);
        }


        /// <summary>
        /// Search courses
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryId">Filter by category</param>
        /// <param name="level">Filter by level</param>
        /// <param name="isFree">Filter by price</param>
        /// <param name="minPrice">Minimum price</param>
        /// <param name="maxPrice">Maximum price</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Search results with facets</returns>

        [HttpGet("search")]
        [AllowAnonymous]
        [EnableRateLimiting("SearchPolicy")]
        [SwaggerOperation(
            Summary = "Search courses",
            Description = "Advanced course search with faceted filtering and relevance scoring."
        )]
        [SwaggerResponse(200, "Success", typeof(SearchCoursesResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(ErrorResponse))]
        [SwaggerResponse(429, "Rate Limit Exceeded", typeof(ErrorResponse))]
        public async Task<ActionResult<SearchCoursesResponse>> SearchCourses(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? categoryId = null,
            [FromQuery] CourseLevel? level = null,
            [FromQuery] bool? isFree = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            CancellationToken cancellationToken = default)
        {
            var query = new SearchCoursesQuery
            {
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = Math.Min(pageSize, 100),
                CategoryId = categoryId.Value,
                Level = level,
                IsFree = isFree,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                RequestedBy = 1
            };

            var result = await _courseService.SearchCoursesAsync(query, cancellationToken);
            return Ok(result);
        }


        /// <summary>
        /// Create a new course
        /// </summary>
        /// <param name="command">Course creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created course information</returns>

        [HttpPost]
        [Authorize(Roles = "1,2")]
        [SwaggerOperation(
            Summary = "Create a new course",
            Description = "Creates a new course. Only instructors and admins can create courses."
        )]
        [SwaggerResponse(201, "Course created successfully", typeof(CreateCourseResponse))]
        [SwaggerResponse(400, "Validation error", typeof(ErrorResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(ErrorResponse))]
        [SwaggerResponse(403, "Forbidden - Insufficient privileges", typeof(ErrorResponse))]
        [SwaggerResponse(429, "Rate Limit Exceeded", typeof(ErrorResponse))]
        public async Task<ActionResult<CreateCourseResponse>> CreateCourse(
            [FromBody] CreateCourseCommand command,
            CancellationToken cancellationToken = default)
        {
            // Set the requesting user
            var commandWithUser = command with { RequestedBy = Convert.ToInt32(GetUserId())};

            var result = await _courseService.CreateCourseAsync(commandWithUser, cancellationToken);

            return CreatedAtAction(
                nameof(GetCourse),
                new { id = result.CourseId },
                result);
        }



        /// <summary>
        /// Publish a course
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Published course information</returns>

        [HttpPost("{id:guid}/publish")]
        [Authorize(Roles = "Instructor,Admin")]
        [SwaggerOperation(
            Summary = "Publish a course",
            Description = "Makes a draft course available to students. Course must have at least one lesson."
        )]
        [SwaggerResponse(200, "Course published successfully", typeof(PublishCourseResponse))]
        [SwaggerResponse(400, "Business rule violation", typeof(ErrorResponse))]
        [SwaggerResponse(404, "Course not found", typeof(ErrorResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(ErrorResponse))]
        [SwaggerResponse(403, "Forbidden", typeof(ErrorResponse))]
        public async Task<ActionResult<PublishCourseResponse>> PublishCourse(
            int id,
            CancellationToken cancellationToken = default)
        {
            var command = new PublishCourseCommand
            {
                ObjectId = id,
                RequestedBy = Convert.ToInt32(GetUserId())
            };

            var result = await _courseService.PublishCourseAsync(command, cancellationToken);
            return Ok(result);
        }


        /// <summary>
        /// Add a lesson to a course
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <param name="command">Lesson data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created lesson information</returns>
        [HttpPost("{id:guid}/lessons")]
        [Authorize(Roles = "Instructor,Admin")]
        [SwaggerOperation(
            Summary = "Add a lesson to a course",
            Description = "Adds a new lesson to an existing course. Course must be in draft status."
        )]
        [SwaggerResponse(201, "Lesson created successfully", typeof(AddLessonResponse))]
        [SwaggerResponse(400, "Validation error", typeof(ErrorResponse))]
        [SwaggerResponse(404, "Course not found", typeof(ErrorResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(ErrorResponse))]
        [SwaggerResponse(403, "Forbidden", typeof(ErrorResponse))]
        public async Task<ActionResult<AddLessonResponse>> AddLesson(
            int id,
            [FromBody] AddLessonCommand command,
            CancellationToken cancellationToken = default)
        {
            var commandWithCourse = command with
            {
                CourseId = id,
                RequestedBy = Convert.ToInt32(GetUserId())
            };

            var result = await _courseService.AddLessonAsync(commandWithCourse, cancellationToken);
            return CreatedAtAction(
                nameof(GetCourse),
                new { id },
                result);
        }

        /// <summary>
        /// Get instructor's own courses
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Instructor's courses</returns>
        
        [HttpGet("my-courses")]
        [Authorize(Roles = "Instructor,Admin")]
        [SwaggerOperation(
            Summary = "Get current user's courses",
            Description = "Retrieves courses created by the current instructor."
        )]
        [SwaggerResponse(200, "Success", typeof(GetCoursesResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(ErrorResponse))]
        public async Task<ActionResult<GetCoursesResponse>> GetMyCourses(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var userEmail = GetUserEmail();
            var result = await _courseService.GetInstructorCoursesAsync(
                userEmail, pageNumber, pageSize, cancellationToken);

            return Ok(result);
        }

        // Helper methods
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   User.FindFirst("user_id")?.Value ??
                   "anonymous";
        }

        private string GetUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value ??
                   User.FindFirst("email")?.Value ??
                   "anonymous@example.com";
        }


    }
}
