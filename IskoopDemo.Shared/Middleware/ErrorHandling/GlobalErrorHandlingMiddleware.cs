using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IskoopDemo.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace IskoopDemo.Shared.Middleware.ErrorHandling
{
    // Global Error Handling Middleware
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;
        private readonly ErrorHandlingOptions _options;

        public GlobalErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalErrorHandlingMiddleware> logger,
            IHostEnvironment environment,
            ErrorHandlingOptions options = null)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
            _options = options ?? new ErrorHandlingOptions();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = CreateErrorResponse(context, exception);

            LogException(exception, response);

            context.Response.StatusCode = response.Status;
            context.Response.ContentType = "application/problem+json";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }

        private ErrorResponse CreateErrorResponse(HttpContext context, Exception exception)
        {
            var response = new ErrorResponse
            {
                Timestamp = DateTime.UtcNow,
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier,
                Extensions = new Dictionary<string, object>()
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    response.Status = (int)HttpStatusCode.BadRequest;
                    response.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
                    response.Title = "Validation Error";
                    response.Detail = "One or more validation errors occurred";
                    response.Errors = validationEx.Errors.Select(e => new ValidationError
                    {
                        Field = e.Field,
                        Message = e.Message,
                        Code = e.Code,
                        AttemptedValue = e.AttemptedValue
                    }).ToList();
                    break;

                case NotFoundException notFoundEx:
                    response.Status = (int)HttpStatusCode.NotFound;
                    response.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
                    response.Title = "Resource Not Found";
                    response.Detail = notFoundEx.Message;
                    response.Extensions["resourceType"] = notFoundEx.ResourceType;
                    response.Extensions["resourceId"] = notFoundEx.ResourceId;
                    break;

                case UnauthorizedException unauthorizedEx:
                    response.Status = (int)HttpStatusCode.Unauthorized;
                    response.Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
                    response.Title = "Unauthorized";
                    response.Detail = unauthorizedEx.Message;
                    if (!string.IsNullOrEmpty(unauthorizedEx.Reason))
                        response.Extensions["reason"] = unauthorizedEx.Reason;
                    break;

                case ForbiddenException forbiddenEx:
                    response.Status = (int)HttpStatusCode.Forbidden;
                    response.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";
                    response.Title = "Forbidden";
                    response.Detail = forbiddenEx.Message;
                    if (!string.IsNullOrEmpty(forbiddenEx.Permission))
                        response.Extensions["requiredPermission"] = forbiddenEx.Permission;
                    break;

                case ConflictException conflictEx:
                    response.Status = (int)HttpStatusCode.Conflict;
                    response.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8";
                    response.Title = "Conflict";
                    response.Detail = conflictEx.Message;
                    if (!string.IsNullOrEmpty(conflictEx.ConflictType))
                        response.Extensions["conflictType"] = conflictEx.ConflictType;
                    if (conflictEx.ConflictingValue != null)
                        response.Extensions["conflictingValue"] = conflictEx.ConflictingValue;
                    break;

                case BusinessException businessEx:
                    response.Status = businessEx.StatusCode;
                    response.Type = $"https://errors.example.com/{businessEx.Code}";
                    response.Title = "Business Rule Violation";
                    response.Detail = businessEx.Message;
                    response.Extensions["errorCode"] = businessEx.Code;
                    foreach (var data in businessEx.Data)
                        response.Extensions[data.Key] = data.Value;
                    break;

                case OperationCanceledException:
                    response.Status = 499; // Client Closed Request
                    response.Type = "https://errors.example.com/request-cancelled";
                    response.Title = "Request Cancelled";
                    response.Detail = "The request was cancelled by the client";
                    break;

                case TimeoutException:
                    response.Status = (int)HttpStatusCode.RequestTimeout;
                    response.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.7";
                    response.Title = "Request Timeout";
                    response.Detail = "The request timed out";
                    break;

                default:
                    response.Status = (int)HttpStatusCode.InternalServerError;
                    response.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
                    response.Title = "Internal Server Error";
                    response.Detail = _environment.IsDevelopment()
                        ? exception.Message
                        : "An unexpected error occurred";

                    if (_environment.IsDevelopment())
                    {
                        response.Extensions["exception"] = exception.GetType().Name;
                        response.Extensions["stackTrace"] = exception.StackTrace;
                        if (exception.InnerException != null)
                        {
                            response.Extensions["innerException"] = new
                            {
                                type = exception.InnerException.GetType().Name,
                                message = exception.InnerException.Message,
                                stackTrace = exception.InnerException.StackTrace
                            };
                        }
                    }
                    break;
            }

            // Add correlation ID if available
            if (context.Items.TryGetValue("CorrelationId", out var correlationId))
            {
                response.Extensions["correlationId"] = correlationId;
            }

            return response;
        }

        private void LogException(Exception exception, ErrorResponse response)
        {
            var logLevel = response.Status >= 500 ? LogLevel.Error : LogLevel.Warning;

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["TraceId"] = response.TraceId,
                ["StatusCode"] = response.Status,
                ["ExceptionType"] = exception.GetType().Name
            }))
            {
                if (logLevel == LogLevel.Error)
                {
                    _logger.LogError(exception,
                        "Unhandled exception occurred. TraceId: {TraceId}, Status: {StatusCode}",
                        response.TraceId, response.Status);
                }
                else
                {
                    _logger.LogWarning(exception,
                        "Handled exception occurred. TraceId: {TraceId}, Status: {StatusCode}, Message: {Message}",
                        response.TraceId, response.Status, exception.Message);
                }
            }
        }
    }
}
