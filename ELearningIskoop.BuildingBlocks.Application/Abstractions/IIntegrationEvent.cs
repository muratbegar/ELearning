using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Application.Abstractions
{
    // Modüller arası iletişim için integration event interface'i
    // Domain event'lerden farklı olarak sistem sınırlarını aşar
    public interface IIntegrationEvent
    {
        // Event'in benzersiz kimliği
        Guid EventId { get; }

        // Event'in oluştuğu zaman
        DateTime OccuredOn { get; }

        // Event'in türü (örn: "UserCreated", "OrderPlaced")
        string EventType { get; }

        // Event'i oluşturan modül
        string Source { get; }

    }
}
