using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Interfaces.Security;
using IskoopDemo.Shared.Models.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TokenValidationResult = IskoopDemo.Shared.Models.Security.TokenValidationResult;

namespace IskoopDemo.Shared.Application.Services.Security
{
    public class JwtTokenService : ITokenService
    {

        private readonly JwtSettings _jwtSettings;
        private readonly ITokenRepository _tokenRepository;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtTokenService(IOptions<JwtSettings> jwtSettings,ITokenRepository tokenRepository)
        {
            _jwtSettings = jwtSettings.Value;
            _tokenRepository = tokenRepository;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public TokenResponse GenerateTokens(IEnumerable<Claim> claims, string userId)
        {
            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken();

            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            // Store refresh token
            _tokenRepository.SaveRefreshTokenAsync(new RefreshTokenEntity
            {
                Token = refreshToken,
                UserId = Convert.ToInt32(userId),
                ExpiresAt = refreshTokenExpiration,
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
            }).GetAwaiter().GetResult();

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };
        }



        public TokenValidationResult ValidateToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
                var validationParameters = GetValidationParameters();

                var principal = _tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    var claims = jwtToken.Claims.ToDictionary(c => c.Type, c => (object)c.Value);

                    return new TokenValidationResult
                    {
                        IsValid = true,
                        Principal = principal,
                        Claims = claims
                    };
                }
                return new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Invalid token format."
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token has expired."
                };
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Invalid token signature"
                };
            }
            catch (Exception ex)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Token validation failed: {ex.Message}"
                };
            }
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new SecurityTokenException("User ID not found in token");
            }

            var storedToken = await _tokenRepository.GetRefreshTokenAsync(request.RefreshToken);
            if (storedToken == null || storedToken.UserId.ToString() != userId || storedToken.IsRevoked)
                throw new SecurityTokenException("Invalid refresh token");

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                throw new SecurityTokenException("Refresh token has expired");

            // Revoke old refresh token
            await _tokenRepository.RevokeRefreshTokenAsync(request.RefreshToken);

            // Generate new tokens
            return GenerateTokens(principal.Claims,userId);
        }

        public async Task RevokeTokenAsync(string token)
        {
            await _tokenRepository.RevokeRefreshTokenAsync(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var validationParameters = GetValidationParameters();
            validationParameters.ValidateLifetime = false; // Don't validate expiration

            try
            {
                var principal = _tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = credentials,
                NotBefore = DateTime.UtcNow,
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }

        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
               ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
               ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = _jwtSettings.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes(_jwtSettings.ClockSkewMinutes),
                RequireExpirationTime = true,
            };
        }
    }
}
