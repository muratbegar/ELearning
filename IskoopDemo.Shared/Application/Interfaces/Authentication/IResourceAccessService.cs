using IskoopDemo.Shared.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces.Authentication
{
    public interface IResourceAccessService
    {
        Task<bool> HasAccessAsync(string userId, string resourceId, ResourceAccessLevel accessLevel);
    }
}
