using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Application.CQRS
{
    public abstract class BaseCommand
    {
        // Command'ı tetikleyen kullanıcı ID'si
        public int? RequestedBy { get; init; }

        // Command'ın oluşturulma zamanı
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Command'ın benzersiz tracking ID'si
        public string TrackingId { get; init; } = Guid.NewGuid().ToString();


        // Sonuç dönen command'lar için base record sınıfı
        //TResponse  Command'ın döneceği sonuç tipi

        
    }
    public abstract record BaseCommand<TResponse> : ICommand<TResponse>
    {
        // Command'ı tetikleyen kullanıcı ID'si
        public int? RequestedBy { get; init; }

        // Command'ın oluşturulma zamanı
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Command'ın benzersiz tracking ID'si
        public string TrackingId { get; init; } = Guid.NewGuid().ToString();

    }
}
