using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Logging
{
    public class AuditEntry
    {
        public string Id { get; set; }
        public string TraceId { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public string ResourceType { get; set; }
        public string ResourceId { get; set; }
        public string RequestBody { get; set; }
        public int? StatusCode { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
