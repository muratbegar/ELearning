using IskoopDemo.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Extensions
{
    public static class ErrorResponseExtensions
    {
        public static BaseResponse ToResponse(this Exception exception, string traceId = null)
        {
            var response = exception switch
            {
                ArgumentException argEx => BaseResponse.Failure(ApiError.BadRequest(argEx.Message)),
                UnauthorizedAccessException => BaseResponse.Failure(ApiError.Unauthorized()),
                KeyNotFoundException => BaseResponse.Failure(ApiError.NotFound()),
                InvalidOperationException invEx => BaseResponse.Failure(ApiError.BadRequest(invEx.Message)),
                NotImplementedException => BaseResponse.Failure(ApiError.ServiceUnavailable("Feature not implemented")),
                TimeoutException => BaseResponse.Failure(ApiError.ServiceUnavailable("Operation timed out")),
                _ => BaseResponse.Failure(ApiError.InternalServerError(exception.Message))
            };

            if (!string.IsNullOrWhiteSpace(traceId))
            {
                response.SetTraceId(traceId);
                if (response.Error != null)
                    response.Error.WithTraceId(traceId);
            }

            return response;
        }

        public static BaseResponse<T> ToResponse<T>(this Exception exception, string traceId = null)
        {
            var baseResponse = exception.ToResponse(traceId);
            return BaseResponse.Failure<T>(baseResponse.Error ?? ApiError.InternalServerError(baseResponse.Message));
        }
    }
}
