using System.Net;
using System.Text.Json;
using ELearningIskoop.BuildingBlocks.Domain;
using FluentValidation;


namespace ELearningIskoop.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next,ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
               await HandleExceptionAsync(context, ex);
            }
        }

        public static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = exception switch
            {
                ValidationException validationEx => new ErrorResponse
                {
                    Title = "Validation Error",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = "One or more validation errors occurred.",
                    Errors = validationEx.Errors.GroupBy(x => x.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray())

                },
                EntityNotFoundException notFoundEx => new ErrorResponse
                {
                    Title = "Resource Not Found",
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = notFoundEx.Message
                },
                DomainException domainEx => new ErrorResponse
                {
                    Title = "Business Rule Violation",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = domainEx.Message,
                    ErrorCode = domainEx.ErrorCode
                },
               
                UnauthorizedAccessException => new ErrorResponse
                {
                    Title = "Unauthorized",
                    Status = (int)HttpStatusCode.Unauthorized,
                    Detail = "You are not authorized to access this resource."
                },
                _ => new ErrorResponse
                {
                    Title = "Internal Server Error",
                    Status = (int)HttpStatusCode.InternalServerError,
                    Detail = "An unexpected error occurred. Please try again later."
                }
            };

            response.StatusCode = errorResponse.Status;

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse);
        }

    }
}
