using IskoopDemo.Shared.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces.Authentication
{
    public interface IAuthenticationProvider
    {
        Task<AuthenticationResult> AuthenticateAsync(LoginRequest request);
        Task<AuthenticationResult> AuthenticateExternalAsync(ExternalLoginInfo externalInfo);
        Task<AuthenticationResult> ValidateTwoFactorAsync(string userId, string code);
        Task SignOutAsync(string userId);
        Task<bool> IsLockedOutAsync(string userId);
        Task ResetAccessFailedCountAsync(string userId);
        Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);
    }
}
