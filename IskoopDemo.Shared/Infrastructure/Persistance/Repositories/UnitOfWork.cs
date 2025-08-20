using IskoopDemo.Shared.Application.Interfaces;
using IskoopDemo.Shared.Domain.Events;
using IskoopDemo.Shared.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly DbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly Dictionary<Type, object> _repositories;
        private bool _disposed=false;

        public UnitOfWork(DbContext context,ICurrentUserService currentUserService,IDateTimeProvider dateTimeProvider,IEventDispatcher eventDispatcher)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _repositories = new Dictionary<Type, object>();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();

                await DispatchDomainEventsAsync();

            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransectionAsync(CancellationToken cancellationToken = default)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransectionAsync(CancellationToken cancellationToken = default)
        {
            await _context.Database.CommitTransactionAsync(cancellationToken);
        }

        public async Task RollbackTransectionAsync(CancellationToken cancellationToken = default)
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IRepository<T>)_repositories[typeof(T)];
            }

            var repository = new Repository<T>(_context);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        private void ApplyAuditInformation()
        {
            var entities = _context.ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            var currentUserId = _currentUserService.UserId;
            var currentTime = _dateTimeProvider.UtcNow;

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((IAuditableEntity)entity.Entity).SetCreatedInfo(currentUserId);
                }

                if (entity.State == EntityState.Modified)
                {
                    ((IAuditableEntity)entity.Entity).SetUpdatedInfo(currentUserId);
                }
            }
        }


        private async Task DispatchDomainEventsAsync()
        {
            var domainEntities = _context.ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _eventDispatcher.DispatchAsync(domainEvent);
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
