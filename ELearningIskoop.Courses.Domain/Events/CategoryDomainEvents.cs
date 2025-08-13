using ELearningIskoop.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Domain.Events
{
    // Yeni kategori oluşturulduğunda tetiklenen domain event
    public record CategoryCreatedDomainEvent(int CategoryId,string Name,string Slug) : DomainEvent;

    // Kategori güncellendiğinde tetiklenen domain event
    public record CategoryUpdatedDomainEvent(int CategoryId,string Name) : DomainEvent;

    // Kategori durumu değiştiğinde tetiklenen domain event
    public record CategoryStatusChangedDomainEvent(int CategoryId,string Name,bool IsActive) : DomainEvent;
}
