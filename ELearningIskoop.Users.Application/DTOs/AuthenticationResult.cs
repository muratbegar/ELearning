using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.DTOs
{
    public class AuthenticationResult
    {
        public bool IsSuccess { get; init; }
        public string? Error { get; init; }
        public int? UserId { get; init; }
        public string? Email { get; init; }
        public string? Name { get;init; }
        public string? Role { get; init; }
        public string? AccessToken { get; init; }
        public string? RefreshToken { get;init; }

        public static AuthenticationResult Success(int userId, string email, string name, string role,
            string accessToken, string refreshToken)
        {
            return new AuthenticationResult
            {
                IsSuccess = true,
                UserId = userId,
                Email = email,
                Name = name,
                Role = role,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public static AuthenticationResult Failed(string error)
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}
