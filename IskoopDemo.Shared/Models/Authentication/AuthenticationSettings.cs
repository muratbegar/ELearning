using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Models.Authentication
{
    public class AuthenticationSettings
    {
        public int MaxFailedAttempts { get; set; } = 5;
        public int LockoutDurationMinutes { get; set; } = 30;
        public bool AllowExternalRegistration { get; set; } = true;
        public bool RequireConfirmedEmail { get; set; } = false;
        public bool RequireUniqueEmail { get; set; } = true;
    }
}
