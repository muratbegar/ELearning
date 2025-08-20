using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Interfaces;
using IskoopDemo.Shared.Domain.Events;

namespace IskoopDemo.Shared.Domain.Entities
{
    public class EventDispatcher : IEventDispatcher
    {

        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task DispatchAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IDomainEvent
        {
            var handlers = GetHandlers<T>();

            var tasks = handlers.Select(handler => handler.Handle(domainEvent, cancellationToken));
            await Task.WhenAll(tasks);
        }

        public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = (IEnumerable<object>)_serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(handlerType));
            if(handlers == null)
            {
                throw new InvalidOperationException($"No handlers found for event type {domainEvent.GetType().Name}");
            }

            var handleMethod = handlerType.GetMethod("Handle");
            var tasks = handlers.Select(handler => (Task)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken }));
            await Task.WhenAll(tasks);
        }

        public async Task DispatchRangeAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            var tasks = domainEvents.Select(domainEvent => DispatchAsync(domainEvent, cancellationToken));
            await Task.WhenAll(tasks);
        }

        private IEnumerable<Application.Interfaces.IDomainEventHandler<T>> GetHandlers<T>() where T : IDomainEvent
        {
            return (IEnumerable<Application.Interfaces.IDomainEventHandler<T>>)_serviceProvider.GetService(typeof(IEnumerable<IDomainEventHandler<T>>));
        }
    }
}
