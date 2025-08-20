using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Logging
{
    public class RequestResponseLoggingOptions
    {
        public bool LogRequestBody { get; set; } = true;
        public bool LogResponseBody { get; set; } = false;
        public bool LogHeaders { get; set; } = true;
        public bool LogResponseHeaders { get; set; } = false;
        public bool MaskSensitiveData { get; set; } = true;
        public bool ExcludeStaticFiles { get; set; } = true;
        public long MaxBodyLogSize { get; set; } = 32768; // 32KB
        public long SlowRequestThresholdMs { get; set; } = 1000;
        public List<string> ExcludePaths { get; set; } = new() { "/health", "/metrics", "/swagger" };
        public List<string> SensitiveHeaders { get; set; } = new() { "Authorization", "Cookie", "X-Api-Key" };
        public List<string> SensitiveDataPatterns { get; set; } = new()
        {
            "password", "token", "secret", "key", "authorization", "cookie", "ssn", "creditcard"
        };
    }
}
