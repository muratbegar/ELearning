using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Domain
{
    // Business rule ihlallerinde fırlatılan exception
    public class BusinessRuleViolationException : DomainException
    {
        public BusinessRuleViolationException(string ruleName, string message)
            : base(message, $"BUSINESS_RULE_VIOLATION_{ruleName.ToUpperInvariant()}")
        {
        }
    }
}
