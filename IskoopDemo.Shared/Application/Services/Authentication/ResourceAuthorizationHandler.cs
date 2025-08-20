using IskoopDemo.Shared.Application.Interfaces.Authentication;
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
    // Resource-based Authorization Handler
    public class ResourceAuthorizationHandler : AuthorizationHandler<ResourceRequirement, IResourceEntity>
    {
        private readonly IResourceAccessService _resourceAccessService;
        private readonly ILogger<ResourceAuthorizationHandler> _logger;

        public ResourceAuthorizationHandler(
            IResourceAccessService resourceAccessService,
            ILogger<ResourceAuthorizationHandler> logger)
        {
            _resourceAccessService = resourceAccessService;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ResourceRequirement requirement,
            IResourceEntity resource)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Authorization failed: User ID not found in claims");
                return;
            }

            var hasAccess = await _resourceAccessService.HasAccessAsync(
                userId,
                resource.Id,
                requirement.AccessLevel);

            if (hasAccess)
            {
                context.Succeed(requirement);
                _logger.LogInformation("Resource authorization succeeded for user {UserId} on resource {ResourceId}",
                    userId, resource.Id);
            }
            else
            {
                _logger.LogWarning("Resource authorization failed for user {UserId} on resource {ResourceId}",
                    userId, resource.Id);
            }
        }
    }
}
