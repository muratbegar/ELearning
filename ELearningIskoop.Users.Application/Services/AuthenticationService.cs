using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.ValueObjects;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Domain.Entitites;
using ELearningIskoop.Users.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ELearningIskoop.Users.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            IUserManager userManager,
            IUserRepository userRepository,
            IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork,
            ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string email, string password, string ipAddress)
        {
            //UserManager ile authentication
            var result = await _userManager.AuthenticateAsync(email, password, ipAddress);
            if (!result.IsSuccess)
            {
                return AuthenticationResult.Failed(result.Error ?? "Authentication failed");
            }

            var user = result.Value;

            // JWT token oluştur
            var roles = await _userManager.GetUserRolesAsync(user.ObjectId);
            var jwtToken = _jwtTokenService.GenerateToken(user, roles);

            // Refresh token oluştur ve kaydet
            var refreshToken = user.AddRefreshToken(ipAddress);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();


            _logger.LogInformation("User {UserId} authenticated from {IpAddress}", user.ObjectId, ipAddress);

            return AuthenticationResult.Success(
                userId: user.ObjectId,
                email: user.Email.Value,
                name: user.Name.FullName,
                role: roles.FirstOrDefault() ?? "Student",
                accessToken: jwtToken,
                refreshToken: refreshToken.Token);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken, string ipAddress)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            if (user == null)
            {
                return AuthenticationResult.Failed("Invalid refresh token");
            }

            var oldToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken);
            if (oldToken == null || !oldToken.IsActive)
            {
                // Token reuse attack detection
                if (oldToken?.IsRevoked == true)
                {
                    user.RevokeAllRefreshToken("Token reuse detected",ipAddress);
                    await _userRepository.UpdateAsync(user);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogWarning("Token reuse attack detected for user {UserId}", user.ObjectId);
                }

                return AuthenticationResult.Failed("Invalid or expired refresh token");
            }
            //Yeni token oluştur
            var roles = await _userManager.GetUserRolesAsync(user.ObjectId);
            var newJwtToken = _jwtTokenService.GenerateToken(user, roles);

            //Eski tokenı iptal et yenisini ekle
            oldToken.Revoke("Replaced by new token",ipAddress,refreshToken);
            var newRefreshToken = user.AddRefreshToken(ipAddress);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return AuthenticationResult.Success(
                userId: user.ObjectId,
                email: user.Email.Value,
                name: user.Name.FullName,
                role: roles.FirstOrDefault() ?? "Student",
                accessToken: newJwtToken,
                refreshToken: newRefreshToken.Token);
        }

        public async Task RevokeTokenAsync(string token, string ipAddress)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(token);
            if (user == null) return;

            user.RevokeRefreshToken(token, "Revoked by user", ipAddress);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Refresh token revoked for user {UserId}", user.ObjectId);
        }

        public async Task<Result> RegisterAsync(RegisterDto dto)
        {
            var createUserDto = new CreateUserDto(
                Email.Create(dto.Email),
                PersonName.Create(dto.FirstName, dto.LastName),
                dto.Password);

            var result = await _userManager.CreateUserAsync(createUserDto);

            if (result.IsSuccess)
            {
                // Send email verification
                // await _emailService.SendEmailVerificationAsync(result.Value);

                _logger.LogInformation("New user registered: {Email}", dto.Email);
            }

            return result.IsSuccess
                ? Result.Success()
                : Result.Failure(result.Error ?? "Registration failed");
        }


    }
}
