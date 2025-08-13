using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Application.Commands.AddLesson;
using ELearningIskoop.Courses.Application.Commands.CreateCourse;
using ELearningIskoop.Courses.Application.Commands.PublishCourse;
using ELearningIskoop.Courses.Application.Queries.GetCourse;
using ELearningIskoop.Courses.Application.Queries.GetCourse.Dto;
using ELearningIskoop.Courses.Application.Queries.SearchCourses;
using ELearningIskoop.Courses.Application.Queries.SearchCourses.Dto;

namespace ELearningIskoop.Courses.Application.Services
{
    public interface ICourseApplicationService
    {
        //Commands
        Task<CreateCourseResponse> CreateCourseAsync(CreateCourseCommand command, CancellationToken cancellationToken);
        Task<PublishCourseResponse> PublishCourseAsync(PublishCourseCommand command, CancellationToken cancellationToken);
        Task<AddLessonResponse> AddLessonAsync(AddLessonCommand command, CancellationToken cancellationToken);

        //Queries
        Task<GetCourseResponse> GetCourseAsync(GetCourseQuery query, CancellationToken cancellationToken);
        Task<List<GetCoursesResponse>> GetCoursesAsync(GetCoursesQuery query, CancellationToken cancellationToken = default);
        Task<SearchCoursesResponse> SearchCoursesAsync(SearchCoursesQuery query, CancellationToken cancellationToken = default);

        //Convenience methods
        Task<GetCourseResponse> GetCourseByIdAsync(int courseId, CancellationToken cancellationToken = default);
        Task<List<GetCoursesResponse>> GetPublishedCoursesAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
        Task<List<GetCoursesResponse>> GetInstructorCoursesAsync(string instructorEmail, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    }
}
