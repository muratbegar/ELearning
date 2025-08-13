using ELearningIskoop.Users.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, IEnumerable<string> roles);
        ClaimsPrincipal? ValidateToken(string token);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
