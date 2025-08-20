using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.ErrorHandling
{
    public class BusinessException : Exception
    {
        public string Code { get; }
        public int StatusCode { get; }
        public Dictionary<string, object> Data { get; }

        public BusinessException(string message, string code = null, int statusCode = 400)
            : base(message)
        {
            Code = code ?? "BUSINESS_ERROR";
            StatusCode = statusCode;
            Data = new Dictionary<string, object>();
        }
    }
}
