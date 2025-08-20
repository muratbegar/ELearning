using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Logging
{
    public class ResponseLog
    {
        public string TraceId { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
