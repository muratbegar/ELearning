using IskoopDemo.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Extensions
{
    public static class ExceptionExtensions
    {
        public static bool IsDomainException(this Exception exception)
        {
            return exception is BaseException;
        }

        public static string GetErrorCode(this Exception exception)
        {
            return exception is BaseException baseException ? baseException.ErrorCode : exception.GetType().Name;
        }

        public static Dictionary<string, object> GetDetails(this Exception exception)
        {
            return exception is BaseException baseException ? baseException.Details : new Dictionary<string, object>();
        }

        public static Exception WithDetail(this Exception exception, string key, object value)
        {
            if (exception is BaseException baseException)
            {
                baseException.WithDetail(key, value);
            }
            return exception;
        }
    }
}
