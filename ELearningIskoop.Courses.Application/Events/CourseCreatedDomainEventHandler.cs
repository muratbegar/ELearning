using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Domain.Events;
using MediatR;
using Serilog;

namespace ELearningIskoop.Courses.Application.Events
{
    // Kurs oluşturulduğunda tetiklenen domain event handler
    public class CourseCreatedDomainEventHandler : INotificationHandler<CourseCreatedDomainEvent>
    {

        private readonly ILogger _logger = Log.ForContext<CourseCreatedDomainEventHandler>();

        public async Task Handle(CourseCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information(
                "Yeni kurs oluşturuldu: {CourseId} - {Title} by {InstructorEmail}",
                notification.CourseId,
                notification.Title,
                notification.InstructorEMail);

            // Burada integration event yayınlanabilir
            // Email bildirimi gönderilebilir
            // Analytics event'i tetiklenebilir

            await Task.CompletedTask;
        }
    }
}
