using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Infrastructure.Email.Models.Enums;

namespace IskoopDemo.Shared.Infrastructure.Email.Models
{
    public class EmailMessage
    {
        public List<string> To { get; set; } = new();
        public List<string> Cc { get; set; } = new();

        public List<string> Bcc { get; set; } = new();
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;

        public List<EmailAttachment> Attachments { get; set; } = new();

        public Dictionary<string, string> Headers { get; set; } = new();

        public EmailPriority Priority { get; set; } = EmailPriority.Normal;

    }
}
