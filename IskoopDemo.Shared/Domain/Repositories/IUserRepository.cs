using IskoopDemo.Shared.Application.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Domain.Entities;

namespace IskoopDemo.Shared.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByExternalProviderAsync(string provider, string providerKey);
        Task<User> CreateAsync(User user);
        Task UpdateLastLoginAsync(string userId, DateTime loginTime, string deviceId);
        Task<LockoutInfo> GetLockoutInfoAsync(string userId);
        Task IncrementFailedAttemptsAsync(string userId);
        Task<int> GetFailedAttemptsAsync(string userId);
        Task ResetFailedAttemptsAsync(string userId);
        Task LockoutUserAsync(string userId, DateTime lockoutEnd);
    }
}
