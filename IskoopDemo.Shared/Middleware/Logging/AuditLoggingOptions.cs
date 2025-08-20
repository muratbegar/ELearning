using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Logging
{
    public class AuditLoggingOptions
    {
        public bool LogRequestBody { get; set; } = true;
        public bool LogResponseBody { get; set; } = false;
        public List<string> ExcludePaths { get; set; } = new();
    }
}
