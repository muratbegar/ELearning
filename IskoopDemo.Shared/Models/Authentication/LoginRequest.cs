using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Models.Authentication
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string TwoFactorCode { get; set; }
        public string DeviceId { get; set; }
    }
}
