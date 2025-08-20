using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Validation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SkipValidationAttribute : Attribute { }

    // Options
    public class RequestSizeOptions
    {
        public long MaxRequestBodySize { get; set; } = 10 * 1024 * 1024; // 10MB default
    }
}
