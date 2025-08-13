using ELearningIskoop.Courses.Domain.Events;
using MediatR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Events
{
    // Kursa ders eklendiğinde tetiklenen domain event handler
    public class LessonAddedDomainEventHandler : INotificationHandler<LessonAddedDomainEvent>
    {
        private readonly ILogger _logger = Log.ForContext<LessonAddedDomainEventHandler>();

        public async Task Handle(LessonAddedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information(
                "Kursa ders eklendi: Course {CourseId} - Lesson {LessonId}: {LessonTitle}",
                notification.CourseId,
                notification.LessonId,
                notification.LessonTitle);

            // Kurs süresini yeniden hesapla (cache'i invalidate et)
            // Kayıtlı öğrencilere yeni ders bildirimi gönder

            await Task.CompletedTask;
        }
    }

}
