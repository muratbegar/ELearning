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
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(string email, string password, string ipAddress);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken, string ipAddress);
        Task RevokeTokenAsync(string token, string ipAddress);
        Task<Result> RegisterAsync(RegisterDto dto);
    }
}
