using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Domain.Entitites;

namespace ELearningIskoop.Users.Application.Services
{
    public interface IUserManager
    {
        Task<Result<User>> CreateUserAsync(CreateUserDto dto);
        Task<Result<User>> GetUserByIdAsync(int userId);
        Task<Result<User>> GetUserByEmailAsync(string email);
        Task<Result<User>> AuthenticateAsync(string email,string password,string ipAddress);

        Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<Result> ResetPasswordAsync(string email, string newPassword);
        Task<Result> VerifyEmailAsync(int userId);
        Task<Result> AssignRoleAsync(int userId, string roleName);
        Task<Result> RemoveRoleAsync(int userId, string roleName);
        Task<bool> IsInRoleAsync(int userId, string roleName);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
    }
}
