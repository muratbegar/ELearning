using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Courses.Domain.Repositories;
using ELearningIskoop.Shared.Domain.Enums;
using ELearningIskoop.Shared.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Commands.AddLesson
{
    public class AddLessonCommandHandler : ICommandHandler<AddLessonCommand,AddLessonResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseRepository _courseRepository;

        public AddLessonCommandHandler(ICourseRepository courseRepository,IUnitOfWork unitOfWork)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }


        public async Task<AddLessonResponse> Handle(AddLessonCommand request, CancellationToken cancellationToken)
        {
            //kursu getir
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

            if (course == null)
            {
                throw new EntityNotFoundException("Course", request.CourseId);
            }

            //ders süresi oluştur
            var duration = Duration.FromMinutes(request.DurationMinutes);

            //kursa ders ekle
            course.AddLesson(
                title: request.Title,
                description: request.Description,
                duration: duration,
                contentType: request.ContentType,
                order: request.Order,
                addedBy: request.RequestedBy
            );

            await _courseRepository.UpdateAsync(course, cancellationToken);
            // Persist changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var addesLesson = course.Lessons.FirstOrDefault(l => l.Order == request.Order);
            if (addesLesson == null)
            {
                throw new InvalidOperationException("Ders eklenemedi");
            }

            // Eklenen dersi bul
            var addedLesson = course.Lessons.FirstOrDefault(l => l.Order == request.Order);
            if (addedLesson == null)
            {
                throw new InvalidOperationException("Ders eklenemedi");
            }

            // İçerik URL'sini ayarla (eğer verilmişse)
            if (!string.IsNullOrEmpty(request.ContentUrl))
            {
                SetLessonContent(addedLesson, request.ContentUrl, request.ContentType, request.RequestedBy);
            }

           

            // Tekrar kaydet
            await _courseRepository.UpdateAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Response oluştur
            return new AddLessonResponse
            {
                LessonId = addedLesson.ObjectId,
                CourseId = course.ObjectId,
                Title = addedLesson.Title,
                Order = addedLesson.Order,
                ContentType = addedLesson.ContentType.GetDescription(),
                Duration = addedLesson.Duration.GetFormattedDuration()
            };
        }

        private void SetLessonContent(Domain.Entities.Lesson lesson, string contentUrl, ContentType contentType,
            int? updatedBy)
        {
            //var result = contentType switch
            //{
            //    ContentType.LiveStream => lesson.SetVideoUrl(contentUrl, updatedBy), void döndürdüğü için bu şekilde kullanılamaz
            //    ContentType.Document => lesson.SetDocumentUrl(contentUrl, updatedBy),
            //    ContentType.Audio => lesson.SetAudioUrl(contentUrl, updatedBy),
            //    ContentType.Quiz => lesson.SetInteractiveContent(contentUrl, updatedBy),
            //};
            switch (contentType)
            {
                case ContentType.LiveStream:
                    lesson.SetVideoUrl(contentUrl, updatedBy);
                    break;
                case ContentType.Document:
                    lesson.SetDocumentUrl(contentUrl, updatedBy);
                    break;
                case ContentType.Audio:
                    lesson.SetAudioUrl(contentUrl, updatedBy);
                    break;
                case ContentType.Quiz:
                    lesson.SetInteractiveContent(contentUrl, updatedBy);
                    break;
            }
        }
    }
}
