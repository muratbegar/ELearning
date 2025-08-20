using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.Entities
{
    public class CurrentUserContext
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
        public List<Claim> Claims { get; set; } = new();
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string TimeZone { get; set; }
        public string PreferredLanguage { get; set; }
        public bool IsAuthenticated { get; set; }

        public string FullName => string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
            ? UserName
            : $"{FirstName} {LastName}".Trim();
    }
}
