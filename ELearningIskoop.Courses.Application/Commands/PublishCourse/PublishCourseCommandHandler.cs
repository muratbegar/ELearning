using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Courses.Domain.Enums;
using ELearningIskoop.Courses.Domain.Repositories;

namespace ELearningIskoop.Courses.Application.Commands.PublishCourse
{
    public class PublishCourseCommandHandler : ICommandHandler<PublishCourseCommand, PublishCourseResponse>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PublishCourseCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PublishCourseResponse> Handle(PublishCourseCommand request, CancellationToken cancellationToken)
        {
            //kurs getir
            var course = await _courseRepository.GetByIdAsync(request.ObjectId, cancellationToken);
            if (course == null)
            {
                throw new EntityNotFoundException("Course",request.ObjectId);
            }

            course.Publish(request.RequestedBy);

            //güncelle
            await _courseRepository.UpdateAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return new PublishCourseResponse
            {
                CourseId = course.ObjectId,
                Title = course.Title,
                Status = course.Status.GetDescription(),
                PublishedAt = course.PublishedAt ?? DateTime.UtcNow,
            };
        }
    }
}
