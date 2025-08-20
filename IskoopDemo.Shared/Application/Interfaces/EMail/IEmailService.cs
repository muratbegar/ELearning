using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Infrastructure.Email.Models;

namespace IskoopDemo.Shared.Application.Interfaces.EMail
{
    internal interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendEmailAsync(EmailMessage message);

        Task SendBulkEmailAsync(IEnumerable<EmailMessage> messages);


        // Template-based emails
        Task SendEmailVerificationAsync(string email,string userName,string verificationLink);
        Task SendPasswordResetAsync(string email, string userName, string resetLink);

        Task SendWelcomeEmailAsync(string email,string userName);
        Task SendAccountLockedAsync(string email, string userName, string reason);
        Task SendPasswordChangedNotificationAsync(string email, string userName);
        Task SendLoginNotificationAsync(string email, string userName, string ipAddress, string deviceInfo);
        Task SendTwoFactorCodeAsync(string email, string userName, string code);

        // Course-related emails
        Task SendCourseEnrollmentConfirmationAsync(string email, string userName, string courseName);
        Task SendCourseCompletionCertificateAsync(string email, string userName, string courseName, string certificateUrl);
        Task SendNewCourseNotificationAsync(string email, string courseName, string instructorName);

        // Admin notifications
        Task SendNewUserRegistrationToAdminAsync(string adminEmail, string newUserEmail, DateTime registrationDate);
        Task SendSuspiciousActivityAlertAsync(string adminEmail, string userEmail, string activityDescription);

    }
}
