using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Validation
{
    public class BusinessValidationContext
    {
        public string UserId { get; set; }
        public string ActionName { get; set; }
        public IDictionary<string, object> ActionArguments { get; set; }
        public string[] Rules { get; set; }
    }
}
