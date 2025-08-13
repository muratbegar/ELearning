using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Domain.Entitites;

namespace ELearningIskoop.Users.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithRolesAsync(int userId, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithTokensAsync(int userId, CancellationToken cancellationToken = default);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken = default);
    }
}
