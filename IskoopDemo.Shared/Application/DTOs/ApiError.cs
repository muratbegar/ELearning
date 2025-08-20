using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.DTOs
{
    public class ApiError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, object> AdditionalInfo { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }
        public string TraceId { get; set; }

        public ApiError()
        {
            Timestamp = DateTime.UtcNow;
            AdditionalInfo = new Dictionary<string, object>();
        }

        public ApiError(string code, string message, int statusCode = 500) : this()
        {
            Code = code;
            Message = message;
            StatusCode = statusCode;
        }

        public ApiError(string code, string message, string details, int statusCode = 500)
            : this(code, message, statusCode)
        {
            Details = details;
        }

        // Factory methods for common errors
        public static ApiError BadRequest(string message, string details = null)
        {
            return new ApiError("BAD_REQUEST", message, details, 400);
        }

        public static ApiError Unauthorized(string message = "Unauthorized access")
        {
            return new ApiError("UNAUTHORIZED", message, (int)HttpStatusCode.Unauthorized);
        }

        public static ApiError Forbidden(string message = "Access forbidden")
        {
            return new ApiError("FORBIDDEN", message, (int)HttpStatusCode.Forbidden);
        }

        public static ApiError NotFound(string message = "Resource not found")
        {
            return new ApiError("NOT_FOUND", message, (int)HttpStatusCode.NotFound);
        }

        public static ApiError Conflict(string message, string details = null)
        {
            return new ApiError("CONFLICT", message, details, (int)HttpStatusCode.Conflict);
        }

        public static ApiError ValidationFailed(string message = "Validation failed")
        {
            return new ApiError("VALIDATION_FAILED", message, (int)HttpStatusCode.BadRequest);
        }

        public static ApiError InternalServerError(string message = "An internal server error occurred")
        {
            return new ApiError("INTERNAL_SERVER_ERROR", message, (int)HttpStatusCode.InternalServerError);
        }

        public static ApiError ServiceUnavailable(string message = "Service temporarily unavailable")
        {
            return new ApiError("SERVICE_UNAVAILABLE", message, (int)HttpStatusCode.ServiceUnavailable);
        }

        public static ApiError TooManyRequests(string message = "Too many requests")
        {
            return new ApiError("TOO_MANY_REQUESTS", message, 429);
        }

        // Business logic errors
        public static ApiError BusinessRuleViolation(string message, string ruleCode = null)
        {
            return new ApiError("BUSINESS_RULE_VIOLATION", message, (int)HttpStatusCode.BadRequest)
            {
                AdditionalInfo = { ["RuleCode"] = ruleCode }
            };
        }

        public static ApiError DomainError(string message, string domainCode = null)
        {
            return new ApiError("DOMAIN_ERROR", message, (int)HttpStatusCode.BadRequest)
            {
                AdditionalInfo = { ["DomainCode"] = domainCode }
            };
        }

        // Add additional information
        public ApiError WithAdditionalInfo(string key, object value)
        {
            AdditionalInfo[key] = value;
            return this;
        }

        public ApiError WithSource(string source)
        {
            Source = source;
            return this;
        }

        public ApiError WithTraceId(string traceId)
        {
            TraceId = traceId;
            return this;
        }
    }
}
