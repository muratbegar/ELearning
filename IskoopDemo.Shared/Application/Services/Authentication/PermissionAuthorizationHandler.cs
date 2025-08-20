using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Services.Authentication
{
    // Authorization Handler
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        public PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var user = context.User;

            if (!user.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Authorization failed: User is not authenticated");
                return Task.CompletedTask;
            }

            // Check for required permission
            var hasPermission = user.Claims.Any(c =>
                c.Type == "permission" && c.Value == requirement.Permission);

            // Check for admin role (bypass)
            var isAdmin = user.IsInRole("Admin") || user.IsInRole("SuperAdmin");

            if (hasPermission || isAdmin)
            {
                context.Succeed(requirement);
                _logger.LogInformation("Authorization succeeded for user {UserId} with permission {Permission}",
                    user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    requirement.Permission);
            }
            else
            {
                _logger.LogWarning("Authorization failed for user {UserId}: Missing permission {Permission}",
                    user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    requirement.Permission);
            }

            return Task.CompletedTask;
        }
    }

}
