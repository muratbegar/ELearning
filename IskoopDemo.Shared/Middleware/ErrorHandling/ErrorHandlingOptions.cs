using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.ErrorHandling
{
    public class ErrorHandlingOptions
    {
        public bool IncludeStackTrace { get; set; }
        public bool IncludeExceptionDetails { get; set; }
        public Dictionary<Type, Func<Exception, ErrorResponse>> CustomHandlers { get; set; }
        public List<string> SensitiveDataPatterns { get; set; }

        public ErrorHandlingOptions()
        {
            CustomHandlers = new Dictionary<Type, Func<Exception, ErrorResponse>>();
            SensitiveDataPatterns = new List<string>
            {
                "password", "token", "secret", "key", "connectionstring"
            };
        }
    }
}
