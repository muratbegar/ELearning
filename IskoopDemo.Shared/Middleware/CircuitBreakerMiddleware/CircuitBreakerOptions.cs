using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.CircuitBreakerMiddleware
{
    public class CircuitBreakerOptions
    {
        public int FailureThreshold { get; set; } = 5;
        public TimeSpan OpenDuration { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan TimeWindow { get; set; } = TimeSpan.FromMinutes(1);
    }
}
