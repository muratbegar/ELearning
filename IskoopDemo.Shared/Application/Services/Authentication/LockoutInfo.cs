using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Services.Authentication
{
    public class LockoutInfo
    {
        public DateTime? LockoutEnd { get; set; }
        public int FailedAttempts { get; set; }
    }
}
