using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Interfaces;

namespace IskoopDemo.Shared.Domain.Exceptions
{
    public class BusinessRuleException : BaseException
    {
        public string RuleCode { get; private set; }
        public string RuleName { get; private set; }
        public Dictionary<string, object> RuleParameters { get; private set; }

        public BusinessRuleException(string message, string ruleCode = null, string ruleName = null)
            : base(message, "BUSINESS_RULE_VIOLATION")
        {
            RuleCode = ruleCode;
            RuleName = ruleName;
            RuleParameters = new Dictionary<string, object>();
        }

        public BusinessRuleException(IBusinessRule brokenRule)
            : base(brokenRule.Message, "BUSINESS_RULE_VIOLATION")
        {
            RuleCode = brokenRule.GetType().Name;
            RuleName = brokenRule.GetType().Name;
            RuleParameters = new Dictionary<string, object>();
        }

        protected BusinessRuleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RuleCode = info.GetString(nameof(RuleCode));
            RuleName = info.GetString(nameof(RuleName));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(RuleCode), RuleCode);
            info.AddValue(nameof(RuleName), RuleName);
        }

        public BusinessRuleException WithParameter(string key, object value)
        {
            RuleParameters[key] = value;
            return this;
        }

        public T GetParameter<T>(string key)
        {
            return RuleParameters.ContainsKey(key) ? (T)RuleParameters[key] : default;
        }

        // Factory methods for common business rules
        public static BusinessRuleException InsufficientPermissions(string operation, string resource = null)
        {
            var message = resource != null
                ? $"Insufficient permissions to {operation} on {resource}"
                : $"Insufficient permissions to {operation}";

            return new BusinessRuleException(message, "INSUFFICIENT_PERMISSIONS")
                .WithParameter("Operation", operation)
                .WithParameter("Resource", resource) as BusinessRuleException;
        }

        public static BusinessRuleException ResourceAlreadyExists(string resourceType, string identifier)
        {
            return new BusinessRuleException(
                $"{resourceType} with identifier '{identifier}' already exists",
                "RESOURCE_ALREADY_EXISTS")
                .WithParameter("ResourceType", resourceType)
                .WithParameter("Identifier", identifier) as BusinessRuleException;
        }

        public static BusinessRuleException InvalidTransition(string currentState, string targetState, string entityType = null)
        {
            var message = entityType != null
                ? $"{entityType} cannot transition from '{currentState}' to '{targetState}'"
                : $"Invalid state transition from '{currentState}' to '{targetState}'";

            return new BusinessRuleException(message, "INVALID_STATE_TRANSITION")
                .WithParameter("CurrentState", currentState)
                .WithParameter("TargetState", targetState)
                .WithParameter("EntityType", entityType) as BusinessRuleException;
        }

        public static BusinessRuleException QuotaExceeded(string quotaType, int currentValue, int maxValue)
        {
            return new BusinessRuleException(
                $"{quotaType} quota exceeded. Current: {currentValue}, Maximum: {maxValue}",
                "QUOTA_EXCEEDED")
                .WithParameter("QuotaType", quotaType)
                .WithParameter("CurrentValue", currentValue)
                .WithParameter("MaxValue", maxValue) as BusinessRuleException;
        }

        public static BusinessRuleException DependencyExists(string dependentResource, string dependency)
        {
            return new BusinessRuleException(
                $"Cannot perform operation because {dependentResource} depends on {dependency}",
                "DEPENDENCY_EXISTS")
                .WithParameter("DependentResource", dependentResource)
                .WithParameter("Dependency", dependency) as BusinessRuleException;
        }
    }
}
