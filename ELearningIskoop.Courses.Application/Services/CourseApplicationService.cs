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
using MediatR;

namespace ELearningIskoop.Courses.Application.Services
{
    public class CourseApplicationService : ICourseApplicationService
    {
        private readonly IMediator _mediator;
        public CourseApplicationService(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #region  Commands

        public async Task<CreateCourseResponse> CreateCourseAsync(CreateCourseCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        public async Task<PublishCourseResponse> PublishCourseAsync(PublishCourseCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        public async Task<AddLessonResponse> AddLessonAsync(AddLessonCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        #endregion

        #region Queries

        public async Task<GetCourseResponse> GetCourseAsync(GetCourseQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        public async Task<List<GetCoursesResponse>> GetCoursesAsync(GetCoursesQuery query, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        public async Task<SearchCoursesResponse> SearchCoursesAsync(SearchCoursesQuery query, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        #endregion

        #region  Convenience methods

        public async Task<GetCourseResponse> GetCourseByIdAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var query = new GetCourseQuery { CourseId = courseId };
            return await _mediator.Send(query, cancellationToken);
        }

        public async Task<List<GetCoursesResponse>> GetPublishedCoursesAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var query = new GetCoursesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                OnlyPublished = true
            };
            return await _mediator.Send(query, cancellationToken);
        }

        public async Task<List<GetCoursesResponse>> GetInstructorCoursesAsync(string instructorEmail, int pageNumber = 1, int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var query = new GetCoursesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                InstructorEmail = instructorEmail,
                OnlyPublished = false
            };
            return await _mediator.Send(query, cancellationToken);
        }

        #endregion


    }
}
