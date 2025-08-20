using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransectionAsync(CancellationToken cancellationToken = default);
        Task CommitTransectionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransectionAsync(CancellationToken cancellationToken = default);

        IRepository<T> Repository<T>() where T : class;

       
    }
}
