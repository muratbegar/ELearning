using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.DTOs
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string TraceId { get; set; }
        public List<ValidationError> ValidationErrors { get; set; }
        public ApiError Error { get; set; }


        public BaseResponse()
        {
            Timestamp = DateTime.UtcNow;
            ValidationErrors = new List<ValidationError>();
        }
        public BaseResponse(bool isSuccess, string message = null) : this()
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        // Success factory methods
        public static BaseResponse Success(string message = "Operation completed successfully")
        {
            return new BaseResponse(true, message);
        }

        public static BaseResponse<T> Success<T>(T data, string message = "Operation completed successfully")
        {
            return new BaseResponse<T>(data, true, message);
        }

        // Failure factory methods
        public static BaseResponse Failure(string message = "Operation failed")
        {
            return new BaseResponse(false, message);
        }

        public static BaseResponse Failure(ApiError error)
        {
            return new BaseResponse(false, error.Message) { Error = error };
        }

        public static BaseResponse Failure(List<ValidationError> validationErrors, string message = "Validation failed")
        {
            return new BaseResponse(false, message) { ValidationErrors = validationErrors };
        }

        public static BaseResponse<T> Failure<T>(string message = "Operation failed")
        {
            return new BaseResponse<T>(default, false, message);
        }

        public static BaseResponse<T> Failure<T>(ApiError error)
        {
            return new BaseResponse<T>(default, false, error.Message) { Error = error };
        }

        public static BaseResponse<T> Failure<T>(List<ValidationError> validationErrors, string message = "Validation failed")
        {
            return new BaseResponse<T>(default, false, message) { ValidationErrors = validationErrors };
        }

        // Validation methods
        public void AddValidationError(string field, string message)
        {
            ValidationErrors.Add(new ValidationError(field, message));
        }

        public void AddValidationErrors(IEnumerable<ValidationError> errors)
        {
            ValidationErrors.AddRange(errors);
        }

        public bool HasValidationErrors => ValidationErrors?.Any() == true;

        public void SetTraceId(string traceId)
        {
            TraceId = traceId;
        }


    }

    // Generic Base Response
    public class BaseResponse<T> : BaseResponse
    {
        public T Data { get; set; }

        public BaseResponse() : base() { }

        public BaseResponse(T data, bool isSuccess, string message = null) : base(isSuccess, message)
        {
            Data = data;
        }

        // Additional success factory methods for generic response
        public static BaseResponse<T> Success(T data, string message = "Operation completed successfully")
        {
            return new BaseResponse<T>(data, true, message);
        }

        // Conversion from non-generic to generic (useful for error responses)
        public static BaseResponse<T> FromBaseResponse(BaseResponse response)
        {
            return new BaseResponse<T>(default, response.IsSuccess, response.Message)
            {
                ValidationErrors = response.ValidationErrors,
                Error = response.Error,
                Timestamp = response.Timestamp,
                TraceId = response.TraceId
            };
        }
    }
}
