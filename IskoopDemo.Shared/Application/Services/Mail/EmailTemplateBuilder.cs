using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Services.Mail
{
    public class EmailTemplateBuilder
    {
        private readonly string _baseUrl;
        private readonly string _logoUrl;
        private readonly string _supportEmail;

        public EmailTemplateBuilder(IConfiguration configuration)
        {
            _baseUrl = configuration["Application:BaseUrl"] ?? "https://elearning.com";
            _logoUrl = configuration["Application:LogoUrl"] ?? $"{_baseUrl}/images/logo.png";
            _supportEmail = configuration["Application:SupportEmail"] ?? "support@elearning.com";
        }

        public string GetBaseTemplate(string title, string content, string? footerText = null)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{title}</title>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            background-color: #ffffff;
            border-radius: 10px;
            padding: 30px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            padding-bottom: 30px;
            border-bottom: 2px solid #f0f0f0;
        }}
        .logo {{
            max-width: 150px;
            height: auto;
        }}
        .content {{
            padding: 30px 0;
        }}
        .button {{
            display: inline-block;
            padding: 12px 30px;
            background-color: #4CAF50;
            color: white !important;
            text-decoration: none;
            border-radius: 5px;
            margin: 20px 0;
            font-weight: bold;
        }}
        .button:hover {{
            background-color: #45a049;
        }}
        .footer {{
            text-align: center;
            padding-top: 30px;
            border-top: 2px solid #f0f0f0;
            font-size: 12px;
            color: #666;
        }}
        .warning {{
            background-color: #fff3cd;
            border: 1px solid #ffc107;
            border-radius: 5px;
            padding: 15px;
            margin: 20px 0;
        }}
        .success {{
            background-color: #d4edda;
            border: 1px solid #28a745;
            border-radius: 5px;
            padding: 15px;
            margin: 20px 0;
        }}
        .info {{
            background-color: #d1ecf1;
            border: 1px solid #17a2b8;
            border-radius: 5px;
            padding: 15px;
            margin: 20px 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='{_logoUrl}' alt='E-Learning Platform' class='logo'>
            <h2>{title}</h2>
        </div>
        <div class='content'>
            {content}
        </div>
        <div class='footer'>
            <p>{footerText ?? "© 2024 E-Learning Platform. All rights reserved."}</p>
            <p>Need help? Contact us at <a href='mailto:{_supportEmail}'>{_supportEmail}</a></p>
            <p><a href='{_baseUrl}/unsubscribe'>Unsubscribe</a> | <a href='{_baseUrl}/preferences'>Email Preferences</a></p>
        </div>
    </div>
</body>
</html>";
        }

        public string BuildEmailVerificationTemplate(string userName, string verificationLink)
        {
            var content = $@"
            <h3>Hello {userName},</h3>
            <p>Welcome to E-Learning Platform! We're excited to have you on board.</p>
            <p>Please verify your email address by clicking the button below:</p>
            <div style='text-align: center;'>
                <a href='{verificationLink}' class='button'>Verify Email Address</a>
            </div>
            <p>Or copy and paste this link into your browser:</p>
            <p style='word-break: break-all; color: #666;'>{verificationLink}</p>
            <div class='warning'>
                <strong>⚠️ Important:</strong> This link will expire in 24 hours for security reasons.
            </div>
            <p>If you didn't create an account with us, please ignore this email.</p>
        ";

            return GetBaseTemplate("Verify Your Email Address", content);
        }

        public string BuildPasswordResetTemplate(string userName, string resetLink)
        {
            var content = $@"
            <h3>Hello {userName},</h3>
            <p>We received a request to reset your password. Click the button below to create a new password:</p>
            <div style='text-align: center;'>
                <a href='{resetLink}' class='button'>Reset Password</a>
            </div>
            <p>Or copy and paste this link into your browser:</p>
            <p style='word-break: break-all; color: #666;'>{resetLink}</p>
            <div class='warning'>
                <strong>⚠️ Security Notice:</strong> This link will expire in 1 hour. If you didn't request this, please ignore this email and your password will remain unchanged.
            </div>
            <p>For security reasons, we recommend:</p>
            <ul>
                <li>Using a strong, unique password</li>
                <li>Enabling two-factor authentication</li>
                <li>Not sharing your password with anyone</li>
            </ul>
        ";

            return GetBaseTemplate("Reset Your Password", content);
        }

        public string BuildWelcomeTemplate(string userName)
        {
            var content = $@"
            <h3>Welcome to E-Learning Platform, {userName}! 🎉</h3>
            <p>Your account has been successfully created and verified. You're all set to start learning!</p>
            
            <div class='success'>
                <strong>✅ Account Verified!</strong> You now have full access to all platform features.
            </div>

            <h4>Here's what you can do next:</h4>
            <ul>
                <li>📚 <a href='{_baseUrl}/courses'>Browse our course catalog</a></li>
                <li>👤 <a href='{_baseUrl}/profile'>Complete your profile</a></li>
                <li>🎯 <a href='{_baseUrl}/learning-path'>Set your learning goals</a></li>
                <li>💬 <a href='{_baseUrl}/community'>Join our community</a></li>
            </ul>

            <div style='text-align: center;'>
                <a href='{_baseUrl}/courses' class='button'>Start Learning Now</a>
            </div>

            <p>Need help getting started? Check out our <a href='{_baseUrl}/help'>help center</a> or reply to this email.</p>
        ";

            return GetBaseTemplate("Welcome to E-Learning Platform!", content);
        }

        public string BuildLoginNotificationTemplate(string userName, string ipAddress, string deviceInfo, DateTime loginTime)
        {
            var content = $@"
            <h3>Hello {userName},</h3>
            <p>We noticed a new login to your account:</p>
            
            <div class='info'>
                <strong>Login Details:</strong><br>
                📅 Time: {loginTime:yyyy-MM-dd HH:mm:ss} UTC<br>
                📍 IP Address: {ipAddress}<br>
                💻 Device: {deviceInfo}
            </div>

            <p>If this was you, no action is needed. You can safely ignore this email.</p>
            
            <div class='warning'>
                <strong>⚠️ Not you?</strong> If you didn't log in, your account may be compromised. 
                Please <a href='{_baseUrl}/reset-password'>reset your password immediately</a> and 
                <a href='{_baseUrl}/security'>review your security settings</a>.
            </div>

            <p>For added security, consider enabling two-factor authentication in your 
            <a href='{_baseUrl}/settings/security'>security settings</a>.</p>
        ";

            return GetBaseTemplate("New Login to Your Account", content);
        }

        public string BuildCourseEnrollmentTemplate(string userName, string courseName, string courseUrl)
        {
            var content = $@"
            <h3>Congratulations {userName}! 🎓</h3>
            <p>You've successfully enrolled in <strong>{courseName}</strong>!</p>
            
            <div class='success'>
                <strong>✅ Enrollment Confirmed!</strong> You now have full access to all course materials.
            </div>

            <h4>What's next?</h4>
            <ul>
                <li>📖 Access your course materials anytime</li>
                <li>💬 Participate in course discussions</li>
                <li>📝 Complete assignments and quizzes</li>
                <li>🏆 Earn your certificate upon completion</li>
            </ul>

            <div style='text-align: center;'>
                <a href='{courseUrl}' class='button'>Go to Course</a>
            </div>

            <p>Tips for success:</p>
            <ul>
                <li>Set a regular study schedule</li>
                <li>Engage with other students in discussions</li>
                <li>Take notes and practice regularly</li>
                <li>Don't hesitate to ask questions</li>
            </ul>

            <p>Happy learning! 🚀</p>
        ";

            return GetBaseTemplate($"Enrolled in {courseName}", content);
        }

    }
}
