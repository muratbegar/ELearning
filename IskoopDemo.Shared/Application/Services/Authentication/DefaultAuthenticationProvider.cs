using IskoopDemo.Shared.Application.Interfaces.Authentication;
using IskoopDemo.Shared.Application.Interfaces.Cryptography;
using IskoopDemo.Shared.Application.Interfaces.Security;
using IskoopDemo.Shared.Domain.Entities;
using IskoopDemo.Shared.Domain.Repositories;
using IskoopDemo.Shared.Models.Authentication;
using IskoopDemo.Shared.Models.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Services.Authentication
{
    public class DefaultAuthenticationProvider : IAuthenticationProvider
    {

        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ITwoFactorService _twoFactorService;
        private readonly ILogger<DefaultAuthenticationProvider> _logger;
        private readonly AuthenticationSettings _settings;

        public DefaultAuthenticationProvider(IUserRepository userRepository,IPasswordHasher passwordHasher,ITokenService service,ITwoFactorService twoFactorService, IOptions<AuthenticationSettings> settings, ILogger<DefaultAuthenticationProvider> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _tokenService = service ?? throw new ArgumentNullException(nameof(service));
            _twoFactorService = twoFactorService ?? throw new ArgumentNullException(nameof(twoFactorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings), "Authentication settings cannot be null.");
        }


        public async Task<AuthenticationResult> AuthenticateAsync(LoginRequest request)
        {
            try
            {
                var user =await _userRepository.GetByUsernameAsync(request.Username);
                if (user == null)
                {
                    _logger.LogWarning("Authentication failed: User {Username} not found", request.Username);
                    return new AuthenticationResult
                    {
                        Succeeded = false,
                        ErrorMessage = "Invalid username or password"
                    };
                }

                if (await IsLockedOutAsync(user.Id.ToString()))
                {
                    _logger.LogWarning("Authentication failed: User {UserId} is locked out", user.Id);
                    return new AuthenticationResult
                    {
                        Succeeded = false,
                        ErrorMessage = "Account is locked. Please try again later."
                    };
                }

                // Verify password
                if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                {
                    await RegisterFailedAttemptAsync(user.Id.ToString());
                    return new AuthenticationResult
                    {
                        Succeeded = false,
                        ErrorMessage = "Invalid username or password"
                    };
                }

                // Check if two-factor is required
                if (user.TwoFactorEnabled)
                {
                    if (string.IsNullOrEmpty(request.TwoFactorCode))
                    {
                        return new AuthenticationResult
                        {
                            Succeeded = false,
                            ErrorMessage = "Two-factor authentication code is required",
                            AdditionalData = new Dictionary<string, object>
                            {
                                ["RequiresTwoFactor"] = true,
                                ["UserId"] = user.Id
                            }
                        };
                    }

                    var twoFactorResult = await ValidateTwoFactorAsync(user.Id, request.TwoFactorCode);
                    if (!twoFactorResult.Succeeded)
                    {
                        return twoFactorResult;
                    }
                }

                //Reset failed attempts on successful login 
                await ResetAccessFailedCountAsync(user.Id.ToString());

                // Generate claims
                var claims = GenerateClaims(user);

                // Generate tokens
                var tokenResponse = _tokenService.GenerateTokens(claims, user.Id);

                // Update last login
                await _userRepository.UpdateLastLoginAsync(user.Id, DateTime.UtcNow, request.DeviceId);

                _logger.LogInformation("User {UserId} authenticated successfully", user.Id);

                return new AuthenticationResult
                {
                    Succeeded = true,
                    Token = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer")),
                    AdditionalData = new Dictionary<string, object>
                    {
                        ["ExpiresAt"] = tokenResponse.AccessTokenExpiration,
                        ["UserId"] = user.Id
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication error for user {Username}", request.Username);
                return new AuthenticationResult
                {
                    Succeeded = false,
                    ErrorMessage = "An error occurred during authentication"
                };
            }
        }

        public async Task<AuthenticationResult> AuthenticateExternalAsync(ExternalLoginInfo externalInfo)
        {
            try
            {
                var user = await _userRepository.GetByExternalProviderAsync(externalInfo.Provider, externalInfo.ProviderKey);
                if (user == null)
                {
                    if (_settings.AllowExternalRegistration)
                    {
                        user = await CreateExternalUserAsync(externalInfo);
                    }
                    else
                    {
                        return new AuthenticationResult
                        {
                            Succeeded = false,
                            ErrorMessage = "External account not linked to any user"
                        };
                    }
                }

                var claims = GenerateClaims(user);
                var tokenResponse = _tokenService.GenerateTokens(claims, user.Id);

                return new AuthenticationResult
                {
                    Succeeded = true,
                    Token = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer")),
                    
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "External authentication error for provider {Provider}", externalInfo.Provider);
                return new AuthenticationResult
                {
                    Succeeded = false,
                    ErrorMessage = "External authentication failed"
                };
            }
        }

        public async Task<AuthenticationResult> ValidateTwoFactorAsync(string userId, string code)
        {
            var isValid = await _twoFactorService.ValidateCodeAsync(userId, code);

            if (!isValid)
            {
                return new AuthenticationResult
                {
                    Succeeded = false,
                    ErrorMessage = "Invalid two-factor authentication code"
                };
            }

            return new AuthenticationResult { Succeeded = true };
        }

        public async Task SignOutAsync(string userId)
        {
            await _tokenService.RevokeTokenAsync(userId);
            _logger.LogInformation("User {UserId} signed out", userId);
        }

        public async Task<bool> IsLockedOutAsync(string userId)
        {
            var lockoutInfo = await _userRepository.GetLockoutInfoAsync(userId);
            return lockoutInfo != null &&
                   lockoutInfo.LockoutEnd > DateTime.UtcNow &&
                   lockoutInfo.FailedAttempts >= _settings.MaxFailedAttempts;
        }

        public async Task ResetAccessFailedCountAsync(string userId)
        {
            await _userRepository.ResetFailedAttemptsAsync(userId);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var refreshRequest = new RefreshTokenRequest
                {
                    RefreshToken = refreshToken
                };

                var tokenResponse = await _tokenService.RefreshTokenAsync(refreshRequest);

                return new AuthenticationResult
                {
                    Succeeded = true,
                    Token = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh failed");
                return new AuthenticationResult
                {
                    Succeeded = false,
                    ErrorMessage = "Token refresh failed"
                };
            }
        }
        private List<Claim> GenerateClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("sub", user.Id),
                new Claim("jti", Guid.NewGuid().ToString())
            };

            // Add roles
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add permissions
            foreach (var permission in user.Permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            return claims;
        }
        private async Task RegisterFailedAttemptAsync(string userId)
        {
            await _userRepository.IncrementFailedAttemptsAsync(userId);

            var attempts = await _userRepository.GetFailedAttemptsAsync(userId);
            if (attempts >= _settings.MaxFailedAttempts)
            {
                await _userRepository.LockoutUserAsync(userId, DateTime.UtcNow.AddMinutes(_settings.LockoutDurationMinutes));
            }
        }
        private async Task<User> CreateExternalUserAsync(ExternalLoginInfo externalInfo)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = externalInfo.Email,
                Email = externalInfo.Email,
                DisplayName = externalInfo.DisplayName,
                IsExternal = true,
                ExternalProvider = externalInfo.Provider,
                ExternalProviderKey = externalInfo.ProviderKey,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
            return user;
        }
    }
}
