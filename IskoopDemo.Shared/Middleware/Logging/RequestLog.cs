using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Logging
{
    public class RequestLog
    {
        public string TraceId { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
        public string RemoteIpAddress { get; set; }
        public string UserAgent { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
