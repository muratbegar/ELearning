using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Infrastructure.Common;

namespace IskoopDemo.Shared.Domain.Exceptions
{
    public class BusinessRuleValidationException : DomainException
    {
        public IBusinessRule BrokenRule { get; }
        public BusinessRuleValidationException(IBusinessRule brokenRule)
            : base(brokenRule.Message)
        {
            BrokenRule = brokenRule ?? throw new ArgumentNullException(nameof(brokenRule));
        }
    }
}
