using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string UserName { get; }
        string Email { get; }
        IEnumerable<string> Roles { get; }
        bool IsAuthenticated { get; }
        string IpAddress { get; }
        string UserAgent { get; }

        bool IsInRole(string role);
        bool HasPermission(string permission);
    }
}
