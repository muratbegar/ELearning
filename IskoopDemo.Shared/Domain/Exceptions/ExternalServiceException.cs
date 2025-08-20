using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.Exceptions
{
    public class ExternalServiceException : BaseException
    {
        public string ServiceName { get; private set; }
        public string ServiceUrl { get; private set; }
        public int? StatusCode { get; private set; }
        public string ServiceResponse { get; private set; }

        public ExternalServiceException(string serviceName, string message)
            : base(message, "EXTERNAL_SERVICE_ERROR")
        {
            ServiceName = serviceName;
        }

        public ExternalServiceException(string serviceName, string serviceUrl, int statusCode, string message, string serviceResponse = null)
            : base(message, "EXTERNAL_SERVICE_ERROR")
        {
            ServiceName = serviceName;
            ServiceUrl = serviceUrl;
            StatusCode = statusCode;
            ServiceResponse = serviceResponse;
        }

        public static ExternalServiceException Timeout(string serviceName, string serviceUrl = null)
        {
            return new ExternalServiceException(serviceName, $"Timeout occurred while calling {serviceName} service")
            {
                ServiceUrl = serviceUrl
            };
        }

        public static ExternalServiceException Unavailable(string serviceName, string serviceUrl = null)
        {
            return new ExternalServiceException(serviceName, $"{serviceName} service is currently unavailable")
            {
                ServiceUrl = serviceUrl
            };
        }
    }
}
