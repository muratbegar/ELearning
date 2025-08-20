using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Validation
{
    public class BusinessValidationResult
    {
        public bool IsValid { get; set; }
        public List<BusinessRuleViolation> Violations { get; set; }
    }
}
