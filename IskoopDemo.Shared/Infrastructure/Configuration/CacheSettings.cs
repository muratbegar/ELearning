using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.Configuration
{
    public class CacheSettings
    {
        public string Provider { get; set; } = "memory";
        public int DefaultExpirationMinutes { get; set; } = 30;
        public bool EnableCompression { get; set; } = false;
        public RedisSettings Redis { get; set; } = new();
    }
}
