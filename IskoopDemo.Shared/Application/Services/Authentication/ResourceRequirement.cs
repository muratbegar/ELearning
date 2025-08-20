using IskoopDemo.Shared.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Services.Authentication
{
    public class ResourceRequirement : IAuthorizationRequirement
    {
        public ResourceAccessLevel AccessLevel { get; }

        public ResourceRequirement(ResourceAccessLevel accessLevel)
        {
            AccessLevel = accessLevel;
        }
    }
}
