using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Infrastructure.Notification.Enums;

namespace IskoopDemo.Shared.Application.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendNotificationAsync(string userId, string title, string message, NotificationType type = NotificationType.Info, CancellationToken cancellationToken = default);
        Task<bool> SendBulkNotificationAsync(IEnumerable<string> userIds, string title, string message, NotificationType type = NotificationType.Info, CancellationToken cancellationToken = default);
        Task<bool> SendPushNotificationAsync(string userId, string title, string message, Dictionary<string, object> data = null, CancellationToken cancellationToken = default);
    }
}
