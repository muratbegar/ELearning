using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Domain
{
    public interface IDomainEvent
    {
        // Event'in benzersiz kimliği
        Guid EventId { get; }

        // Event'in oluşturulma zamanı
        DateTime OccurredOn { get; }

        //Event Tipi
        string EventType { get; }
    }
}
