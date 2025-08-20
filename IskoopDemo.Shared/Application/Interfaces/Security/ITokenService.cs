using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Models.Security;

namespace IskoopDemo.Shared.Application.Interfaces.Security
{
    public interface ITokenService
    {
        TokenResponse GenerateTokens(IEnumerable<Claim> claims, string userId);
        TokenValidationResult ValidateToken(string token);
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task RevokeTokenAsync(string token);
        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
