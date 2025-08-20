using IskoopDemo.Shared.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces.Security
{
    public interface ITokenRepository
    {
        Task SaveRefreshTokenAsync(RefreshTokenEntity token);
        Task<RefreshTokenEntity> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
        Task RevokeAllUserTokensAsync(string userId);
        Task<IEnumerable<RefreshTokenEntity>> GetActiveUserTokensAsync(string userId);
        Task CleanupExpiredTokensAsync();
    }
}
