using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopLearning.Shared.Interfaces
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTime OccurredOn { get; }
        string EventType { get; }
        string? AggregateId { get; } // CQRS
        string? AggregateType { get; } // CQRS
        int Version { get; } 
        IDictionary<string, object> Metadata { get; } 
    }
}
