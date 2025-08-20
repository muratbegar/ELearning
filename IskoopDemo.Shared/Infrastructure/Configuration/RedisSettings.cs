using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.Configuration
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; }
        public string InstanceName { get; set; } = "ELearning";
        public int Database { get; set; } = 0;
        public bool AbortOnConnectFail { get; set; } = false;
    }
}
