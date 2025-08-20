using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.Exceptions
{
    public class AuthorizationException : BaseException
    {
        public string UserId { get; private set; }
        public string Resource { get; private set; }
        public string Operation { get; private set; }
        public List<string> RequiredRoles { get; private set; }
        public List<string> RequiredPermissions { get; private set; }

        public AuthorizationException(string message = "Access denied")
            : base(message, "AUTHORIZATION_ERROR")
        {
            RequiredRoles = new List<string>();
            RequiredPermissions = new List<string>();
        }

        public AuthorizationException(string userId, string resource, string operation)
            : base($"User '{userId}' is not authorized to {operation} on {resource}", "AUTHORIZATION_ERROR")
        {
            UserId = userId;
            Resource = resource;
            Operation = operation;
            RequiredRoles = new List<string>();
            RequiredPermissions = new List<string>();
        }

        public AuthorizationException WithRequiredRole(string role)
        {
            RequiredRoles.Add(role);
            return this;
        }

        public AuthorizationException WithRequiredPermission(string permission)
        {
            RequiredPermissions.Add(permission);
            return this;
        }
    }
}
