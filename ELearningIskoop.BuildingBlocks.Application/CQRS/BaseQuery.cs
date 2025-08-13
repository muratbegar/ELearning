using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Application.CQRS
{
    // Query'ler için base record sınıfı
    public abstract record BaseQuery<TResponse> : IQuery<TResponse>
    {
        // Query'yi tetikleyen kullanıcı ID'si
        public int? RequestedBy { get; init; }

        // Query'nin oluşturulma zamanı
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Query'nin benzersiz tracking ID'si
        public Guid TrackingId { get; init; } = Guid.NewGuid();
    }
}
