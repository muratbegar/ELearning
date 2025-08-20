using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Models.Authentication
{
    public class ExternalLoginInfo
    {
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public Dictionary<string, string> Claims { get; set; }
    }
}
