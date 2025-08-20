using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Models.Authentication
{
    public class AuthenticationResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; }
    }
}
