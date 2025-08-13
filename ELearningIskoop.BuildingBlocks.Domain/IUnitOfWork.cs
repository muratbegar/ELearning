using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Domain
{
    // Unit of Work pattern interface'i
    // Transaction yönetimi için kullanılır
    public interface IUnitOfWork : IDisposable
    {

        //Değişkenleri veritabanına commit eder
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // Transaction başlatır
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        // Transaction commit eder
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        // Transaction rollback eder
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
