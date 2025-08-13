using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Domain.Entitites;
using ELearningIskoop.Users.Domain.Repositories;
using ELearningIskoop.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ELearningIskoop.Users.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly UserDbContext _context;

        public RoleRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpperInvariant(), cancellationToken);
        }

        public async Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Role>> GetRolesWithPermissionsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Include(r => r.Permissions)
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.ObjectId == id, cancellationToken);
        }

        public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Role>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .OrderBy(r => r.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Role>> FindAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Role> GetSingleAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(predicate, cancellationToken);

            if (role == null)
                throw new EntityNotFoundException("Role", "predicate");

            return role;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Roles.AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles.CountAsync(cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Roles.CountAsync(predicate, cancellationToken);
        }

        public async Task<Role> AddAsync(Role entity, CancellationToken cancellationToken = default)
        {
            _context.Roles.Add(entity);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<Role> entities, CancellationToken cancellationToken = default)
        {
            _context.Roles.AddRange(entities);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            _context.Roles.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Role entity, CancellationToken cancellationToken = default)
        {
            _context.Roles.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var role = await GetByIdAsync(id, cancellationToken);
            if (role != null)
            {
                _context.Roles.Remove(role);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<Role> entities, CancellationToken cancellationToken = default)
        {
            _context.Roles.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Role>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Where(r => r.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Role>> GetDeletedAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .IgnoreQueryFilters()
                .Where(r => r.IsDeleted)
                .ToListAsync(cancellationToken);
        }
    }
}
