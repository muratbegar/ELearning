using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.Enums;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Domain.Entitites;
using ELearningIskoop.Users.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ELearningIskoop.Users.Application.Services
{
    public class UserManager : IUserManager
    {

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserManager> _logger;

        public UserManager(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork, ILogger<UserManager> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<User>> CreateUserAsync(CreateUserDto dto)
        {
            try
            {
                //Email benzersizlik kontrolü
                var existingUser = await _userRepository.GetByEmailAsync(dto.Email.Value);
                if (existingUser == null)
                {
                    return Result.Failure<User>("Email is already registered");
                }

                //Kullanıcı oluştur
                var user = User.Create(dto.Email, dto.Name, dto.Password);

                //Default role ata
                var defaultRole = await _roleRepository.GetByNameAsync(enUserRole.Student.ToString());
                if (defaultRole != null)
                {
                    user.AssignRole(defaultRole);
                }

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User created: {UserId} - {Email}", user.ObjectId, user.Email.Value);

                return Result.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Email}", dto.Email.Value);
                return Result.Failure<User>("An error occurred while creating user");
            }
        }

        public async Task<Result<User>> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdWithRolesAsync(userId);
                return user != null ? Result.Success(user) : Result.Failure<User>("User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User not found");
                return Result.Failure<User>("An error occurred while creating user");
            }
           
        }

        public async Task<Result<User>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                return user != null ? Result.Success(user) : Result.Failure<User>("User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User not found");
                return Result.Failure<User>("An error occurred while creating user");
            }

           
        }

        public async Task<Result<User>> AuthenticateAsync(string email, string password, string ipAddress)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    // Timing attack prevention
                    await Task.Delay(Random.Shared.Next(100, 500));
                    return Result.Failure<User>("Invalid credentials");

                }

                var loginResult = user.AttemptLogin(password, ipAddress);
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                if (!loginResult.IsSuccess)
                {
                    return Result.Failure<User>(loginResult.Error ?? "Authentication failed");
                }

                return Result.Success(user);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User Invalid credentials");
                return Result.Failure<User>("An error occurred while creating user");
            }
           
        }

        public async Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            try
            {
                user.ChangePassword(currentPassword,newPassword);
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (BusinessRuleViolationException ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            user.ResetPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> VerifyEmailAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            try
            {
                user.VerifyEmail();
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> AssignRoleAsync(int userId, string roleName)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role == null)
            {
                return Result.Failure("Role not found");
            }

            user.AssignRole(role);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> RemoveRoleAsync(int userId, string roleName)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role == null)
            {
                return Result.Failure("Role not found");
            }
            
            user.RemoveRule(role.ObjectId);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<bool> IsInRoleAsync(int userId, string roleName)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            return user?.HasRole(roleName) ?? false;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null)
            {
                return Enumerable.Empty<string>();
            }

            return user.UserRoles.Select(ur => ur.Role.Name);
        }
    }
}
