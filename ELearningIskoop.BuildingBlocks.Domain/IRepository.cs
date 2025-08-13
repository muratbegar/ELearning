using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Domain
{
    // Repository pattern için temel interface
    // Domain katmanında data access abstraksiyonu sağlar
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        // ID'ye göre entity getirir
        Task<TEntity?> GetByIdAsync(int id,CancellationToken cancellationToken=default);

        // Tüm entity'leri getirir
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        // Sayfalama ile entity'leri getirir
        Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        // Predicate ile entity'leri filtreler
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        // Tek entity getirir (bulunamazsa exception fırlatır)
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        // Entity varlığını kontrol eder
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        // Toplam kayıt sayısını getirir
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        // Predicate ile toplam kayıt sayısını getirir
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        // Yeni entity ekler
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        // Çoklu entity ekler
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        // Entity günceller
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        // Entity siler (hard delete)
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        //// ID ile entity siler (hard delete)
        //Task DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default);

        // Çoklu entity siler
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        //ObjecyID'li entity'ler için repository interface
        //public interface IRepository<TEntity> : IRepository<TEntity, int>
        //    where TEntity : BaseEntity
        //{
        //}

        Task<IEnumerable<TEntity>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetDeletedAsync(CancellationToken cancellationToken = default);

    }
}
