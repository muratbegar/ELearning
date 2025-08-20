using IskoopDemo.Shared.Infrastructure.Email.Models;
using IskoopDemo.Shared.Infrastructure.Email.Models.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Interfaces.EMail;

namespace IskoopDemo.Shared.Application.Services.Mail
{
    public class SmtpEmailService : IEmailService
    {

        private readonly EmailSettings _emailSettings;
        private readonly EmailTemplateBuilder _emailTemplateBuilder;
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public SmtpEmailService(IOptions<EmailSettings> emailSettings, EmailTemplateBuilder templateBuilder,ILogger<SmtpEmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _emailTemplateBuilder = templateBuilder;
            _logger = logger;



            // Configure retry policy with Polly
            // Configure retry policy with Polly
            _retryPolicy = Policy
                .Handle<SmtpException>()
                .Or<IOException>()
                .WaitAndRetryAsync(
                    _emailSettings.MaxRetryAttempts,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning("Email send attempt {RetryCount} failed. Retrying in {Timespan}ms",
                            retryCount, timespan.TotalMilliseconds);
                    });

        }


        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            var message = new EmailMessage
            {
                To = new List<string> { to },
                Subject = subject,
                Body = body,
                IsHtml = isHtml
            };

            await SendEmailAsync(message);
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    using (var mailMessage = CreateMailMessage(message))
                    {
                        using (var smtpClient = CreateSmtpClient())
                        {
                            await smtpClient.SendMailAsync(mailMessage);

                            _logger.LogInformation("Email sent successfully to {Recipients}",
                                string.Join(", ", message.To));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipients} after {MaxRetries} attempts",
                    string.Join(", ", message.To), _emailSettings.MaxRetryAttempts);

                // Store in queue for later retry or notify admin
                await StoreFailedEmailAsync(message, ex.Message);
                throw;
            }
        }

        public async Task SendBulkEmailAsync(IEnumerable<EmailMessage> messages)
        {
            var tasks = messages.Select(SendEmailAsync);
            await Task.WhenAll(tasks);
        }

        public async Task SendEmailVerificationAsync(string email, string userName, string verificationLink)
        {
           var body = _emailTemplateBuilder.BuildEmailVerificationTemplate(userName, verificationLink);
            var subject = "Please verify your email address";
            var message = new EmailMessage
            {
                To = new List<string> { email },
                Subject = subject,
                Body = body,
                IsHtml = true
            };
            await SendEmailAsync(message);
        }

        public async Task SendPasswordResetAsync(string email, string userName, string resetLink)
        {
            var body = _emailTemplateBuilder.BuildPasswordResetTemplate(userName, resetLink);
            var subject = "Password Reset Request";
            var message = new EmailMessage
            {
                To = new List<string> { email },
                Subject = subject,
                Body = body,
                IsHtml = true
            };
            await SendEmailAsync(message);
        }

        public async Task SendWelcomeEmailAsync(string email, string userName)
        {
            var body = _emailTemplateBuilder.BuildWelcomeTemplate(userName);
            await SendEmailAsync(email, "Welcome to E-Learning Platform!", body);
        }

        public async Task SendAccountLockedAsync(string email, string userName, string reason)
        {
            var content = $@"
            <h3>Hello {userName},</h3>
            <p>Your account has been temporarily locked for security reasons.</p>
            <p><strong>Reason:</strong> {reason}</p>
            <p>To unlock your account, please contact our support team or reset your password.</p>
        ";

            var body = _emailTemplateBuilder.GetBaseTemplate("Account Locked", content);
            await SendEmailAsync(email, "Account Security Alert", body);
        }

        public async Task SendPasswordChangedNotificationAsync(string email, string userName)
        {
            var content = $@"
            <h3>Hello {userName},</h3>
            <p>Your password has been successfully changed.</p>
            <p>If you didn't make this change, please contact our support team immediately.</p>
        ";

            var body = _emailTemplateBuilder.GetBaseTemplate("Password Changed", content);
            await SendEmailAsync(email, "Password Changed Successfully", body);
        }

        public async Task SendLoginNotificationAsync(string email, string userName, string ipAddress, string deviceInfo)
        {
            var body = _emailTemplateBuilder.BuildLoginNotificationTemplate(userName, ipAddress, deviceInfo, DateTime.UtcNow);
            await SendEmailAsync(email, "New Login to Your Account", body);
        }

        public async  Task SendTwoFactorCodeAsync(string email, string userName, string code)
        {
            var content = $@"
            <h3>Hello {userName},</h3>
            <p>Your two-factor authentication code is:</p>
            <div style='text-align: center; font-size: 32px; font-weight: bold; letter-spacing: 5px; margin: 30px 0;'>
                {code}
            </div>
            <p>This code will expire in 5 minutes.</p>
        ";

            var body = _emailTemplateBuilder.GetBaseTemplate("Your Login Code", content);
            await SendEmailAsync(email, $"Your Login Code: {code}", body);
        }

        public async Task SendCourseEnrollmentConfirmationAsync(string email, string userName, string courseName)
        {
            var courseUrl = $"{_emailSettings.FromEmail}/courses/{courseName.ToLower().Replace(" ", "-")}";
            var body = _emailTemplateBuilder.BuildCourseEnrollmentTemplate(userName, courseName, courseUrl);
            await SendEmailAsync(email, $"Enrolled in {courseName}", body);
        }

        public async Task SendCourseCompletionCertificateAsync(string email, string userName, string courseName, string certificateUrl)
        {
            var content = $@"
            <h3>Congratulations {userName}! 🎉🎓</h3>
            <p>You've successfully completed <strong>{courseName}</strong>!</p>
            <div class='success'>
                <strong>🏆 Certificate Earned!</strong> Download your certificate below.
            </div>
            <div style='text-align: center;'>
                <a href='{certificateUrl}' class='button'>Download Certificate</a>
            </div>
        ";

            var body = _emailTemplateBuilder.GetBaseTemplate($"Certificate: {courseName}", content);
            await SendEmailAsync(email, $"Congratulations! You've completed {courseName}", body);
        }

        public async Task SendNewCourseNotificationAsync(string email, string courseName, string instructorName)
        {
            var content = $@"
            <h3>New Course Available!</h3>
            <p>A new course has been added that might interest you:</p>
            <h4>{courseName}</h4>
            <p>Instructor: {instructorName}</p>
            <div style='text-align: center;'>
                <a href='#' class='button'>View Course</a>
            </div>
        ";

            var body = _emailTemplateBuilder.GetBaseTemplate("New Course Available", content);
            await SendEmailAsync(email, $"New Course: {courseName}", body);
        }

        public async Task SendNewUserRegistrationToAdminAsync(string adminEmail, string newUserEmail, DateTime registrationDate)
        {
            var content = $@"
            <h3>New User Registration</h3>
            <p>A new user has registered on the platform:</p>
            <ul>
                <li>Email: {newUserEmail}</li>
                <li>Registration Date: {registrationDate:yyyy-MM-dd HH:mm:ss}</li>
            </ul>
        ";

            var body = _emailTemplateBuilder.GetBaseTemplate("New User Registration", content);
            await SendEmailAsync(adminEmail, "New User Registration", body);
        }

        public async Task SendSuspiciousActivityAlertAsync(string adminEmail, string userEmail, string activityDescription)
        {
            var content = $@"
            <h3>⚠️ Suspicious Activity Detected</h3>
            <p>Suspicious activity has been detected for user: {userEmail}</p>
            <p><strong>Activity:</strong> {activityDescription}</p>
            <p>Please review this activity in the admin panel.</p>
        ";

            var body = _emailTemplateBuilder.GetBaseTemplate("Security Alert", content);
            await SendEmailAsync(adminEmail, "Security Alert: Suspicious Activity", body);
        }

        private MailMessage CreateMailMessage(EmailMessage message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = message.IsHtml,
                Priority = ConvertPriority(message.Priority)
            };

            foreach (var to in message.To)
            {
                mailMessage.To.Add(to);
            }

            foreach (var cc in message.Cc)
            {
                mailMessage.CC.Add(cc);
            }

            foreach (var bcc in message.Bcc)
            {
                mailMessage.Bcc.Add(bcc);
            }

            if (!string.IsNullOrEmpty(_emailSettings.ReplyToEmail))
            {
                mailMessage.ReplyToList.Add(_emailSettings.ReplyToEmail);
            }

            foreach (var attachment in message.Attachments)
            {
                var stream = new MemoryStream(attachment.Content);
                mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
            }

            return mailMessage;
        }
        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = _emailSettings.SmtpServer,
                Port = _emailSettings.SmtpPort,
                EnableSsl = _emailSettings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                Timeout = 30000 // 30 seconds
            };
        }
        private MailPriority ConvertPriority(EmailPriority priority)
        {
            return priority switch
            {
                EmailPriority.High => MailPriority.High,
                EmailPriority.Low => MailPriority.Low,
                _ => MailPriority.Normal
            };
        }

        private async Task StoreFailedEmailAsync(EmailMessage message, string errorMessage)
        {
            // Store failed emails in database or file system for retry
            // This is a placeholder - implement based on your requirements
            _logger.LogError("Failed email stored for retry: {Subject} to {Recipients}. Error: {Error}",
                message.Subject, string.Join(", ", message.To), errorMessage);
        }
    }
}
