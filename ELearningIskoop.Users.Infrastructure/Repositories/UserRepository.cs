using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.Enums;
using ELearningIskoop.Users.Domain.Entitites;
using ELearningIskoop.Users.Domain.Repositories;
using ELearningIskoop.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ELearningIskoop.Users.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.ObjectId == id, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .OrderBy(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .OrderBy(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<User> GetSingleAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(predicate, cancellationToken);

            if (user == null)
                throw new EntityNotFoundException("User", "predicate");

            return user;
        }

        public async Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users.CountAsync(cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Users.CountAsync(predicate, cancellationToken);
        }

        public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            _context.Users.Add(entity);
            await Task.CompletedTask;
        }

        public async Task AddRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
        {
            _context.Users.AddRange(entities);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await GetByIdAsync(id, cancellationToken);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }

        public async Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
        {
            _context.Users.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
        {
            _context.Users.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<User>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Where(u => u.Status ==  UserStatus.Active)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetDeletedAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .IgnoreQueryFilters()
                .Where(u => u.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<User?> GetByIdWithRolesAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.ObjectId == userId, cancellationToken);
        }

        public async Task<User?> GetByIdWithTokensAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.ObjectId == userId, cancellationToken);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.RefreshTokens)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken), cancellationToken);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
        {
            return !await _context.Users
                .AnyAsync(u => u.Email.Value == email, cancellationToken);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken = default)
        {
            return !await _context.Users
                .AnyAsync(u => u.Username == username, cancellationToken);
        }
    }
}
