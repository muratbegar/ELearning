using ELearningIskoop.Courses.Domain.Events;
using MediatR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Events
{
    // Kurs yayınlandığında tetiklenen domain event handler
    public class CoursePublishedDomainEventHandler : INotificationHandler<CoursePublishedDomainEvent>
    {
        private readonly ILogger _logger = Log.ForContext<CoursePublishedDomainEventHandler>();

        public async Task Handle(CoursePublishedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information(
                "Kurs yayınlandı: {CourseId} - {Title} by {InstructorEmail}",
                notification.CourseId,
                notification.Title,
                notification.InstructorEMail);

            // Integration event yayınla (diğer modüller bilgilendirilsin)
            // Notification gönder (öğrencilere yeni kurs bildirimi)
            // Search index güncelle
            // Cache invalidate et

            await Task.CompletedTask;
        }
    }
}
