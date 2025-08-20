using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.Common
{
    public class IntegrationEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public string EventType { get; }

        protected IntegrationEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            EventType = GetType().Name;
        }
    }
}
