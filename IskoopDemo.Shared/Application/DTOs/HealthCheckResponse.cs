using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.DTOs
{
    public class HealthCheckResponse : BaseResponse
    {
        public string ServiceName { get; set; }
        public string Version { get; set; }
        public string Environment { get; set; }
        public Dictionary<string, ServiceHealth> Dependencies { get; set; }

        public HealthCheckResponse() : base()
        {
            Dependencies = new Dictionary<string, ServiceHealth>();
        }

        public static HealthCheckResponse Healthy(string serviceName, string version, string environment = null)
        {
            return new HealthCheckResponse
            {
                IsSuccess = true,
                Message = "Service is healthy",
                ServiceName = serviceName,
                Version = version,
                Environment = environment
            };
        }

        public static HealthCheckResponse Unhealthy(string serviceName, string version, string reason, string environment = null)
        {
            return new HealthCheckResponse
            {
                IsSuccess = false,
                Message = $"Service is unhealthy: {reason}",
                ServiceName = serviceName,
                Version = version,
                Environment = environment
            };
        }
    }
}
