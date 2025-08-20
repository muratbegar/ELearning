using IskoopDemo.Shared.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Interfaces;

namespace IskoopDemo.Shared.Infrastructure.Common
{
    public class DomainEvent : IDomainEvent
    {
        public Guid EventId { get; }
        public DateTime OccurredOn { get; }
        public string EventType { get; }

        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            EventType = GetType().Name;
        }

    }
}
