using ELearningIskoop.Users.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);

        Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Role>> GetRolesWithPermissionsAsync(CancellationToken cancellationToken = default);

        Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Role>> GetPagedAsync(int pageNumber, int pageSize,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Role>> FindAsync(Expression<Func<Role, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<Role> GetSingleAsync(Expression<Func<Role, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default);

        Task<int> CountAsync(CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default);


        Task<Role> AddAsync(Role entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<Role> entities, CancellationToken cancellationToken = default);
        Task UpdateAsync(Role entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Role entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task DeleteRangeAsync(IEnumerable<Role> entities, CancellationToken cancellationToken = default);

        Task<IEnumerable<Role>> GetActiveAsync(CancellationToken cancellationToken = default);


        Task<IEnumerable<Role>> GetDeletedAsync(CancellationToken cancellationToken = default);

    }
}
