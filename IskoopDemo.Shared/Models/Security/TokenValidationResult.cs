using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Models.Security
{
    public class TokenValidationResult
    {
        public bool IsValid { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, object> Claims { get; set; }
    }
}
