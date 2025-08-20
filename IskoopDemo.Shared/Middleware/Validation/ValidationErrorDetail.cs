using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Validation
{
    public class ValidationErrorDetail
    {
        public string Field { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public object AttemptedValue { get; set; }
        public string Severity { get; set; }
    }
}
