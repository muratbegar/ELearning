using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Validation
{
    public class BusinessRuleViolation
    {
        public string Rule { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public Dictionary<string, object> Context { get; set; }
    }
}
