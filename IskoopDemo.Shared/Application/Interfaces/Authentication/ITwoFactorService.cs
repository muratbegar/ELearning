using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces.Authentication
{
    public interface ITwoFactorService
    {
        Task<bool> ValidateCodeAsync(string userId, string code);
        Task<string> GenerateCodeAsync(string userId);
        Task<bool> EnableTwoFactorAsync(string userId);
        Task<bool> DisableTwoFactorAsync(string userId);
    }
}
