using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.Email.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = string.Empty ;
        public string SmtpPassword { get; set;} = string.Empty ;
        public bool EnableSsl { get; set; } = true ;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string ReplyToEmail { get; set; } = string.Empty;

        public int MaxRetryAttempts { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 5;
        public bool EnableEmailLogging { get; set; } = true;
        public string? EmailLogPath { get; set; }




        // SendGrid settings (alternative)
        public string? SendGridApiKey { get; set; }
        public bool UseSendGrid { get; set; } = false;


        // AWS SES settings (alternative)
        public string? AwsAccessKey { get; set; }
        public string? AwsSecretKey { get; set; }
        public string? AwsRegion { get; set; }
        public bool UseAwsSes { get; set; } = false;

    }
}
