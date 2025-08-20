using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Validation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ValidateBusinessRulesAttribute : Attribute
    {
        public string[] Rules { get; }

        public ValidateBusinessRulesAttribute(params string[] rules)
        {
            Rules = rules;
        }
    }
}
