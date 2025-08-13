using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Application.Abstractions
{
    // Integration event'ler için base record
    public abstract record IntegrationEvent : IIntegrationEvent
    {
        protected IntegrationEvent(string source)
        {
            EventId = Guid.NewGuid();
            OccuredOn = DateTime.UtcNow;
            EventType = GetType().Name;
            Source = source;
        }

        public Guid EventId { get; init; }
        public DateTime OccuredOn { get; init; }
        public string EventType { get; init; }
        public string Source { get; init; }

        
    }
}
