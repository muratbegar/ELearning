using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Interfaces;

namespace IskoopDemo.Shared.Domain.Events
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IDomainEvent;
        Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
        Task DispatchRangeAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
