using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.DTOs
{
    public class ServiceHealth
    {
        public bool IsHealthy { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public DateTime LastChecked { get; set; }

        public ServiceHealth(bool isHealthy, string status, string message = null, TimeSpan responseTime = default)
        {
            IsHealthy = isHealthy;
            Status = status;
            Message = message;
            ResponseTime = responseTime;
            LastChecked = DateTime.UtcNow;
        }
    }
}
