using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BaseAPI.Middleware.JWTMidlleware
{
    public class AnyPolicyHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User?.Identity == null || !context.User.Identity.IsAuthenticated)
                return Task.CompletedTask;

            var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(roleClaim) &&
                roleClaim.Equals("Super", StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var permissionsClaim = context.User.FindFirst("permissions")?.Value;
            if (string.IsNullOrEmpty(permissionsClaim))
                return Task.CompletedTask;

            var permissions = permissionsClaim.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var policyParts = requirement.Permission.Split('|', StringSplitOptions.RemoveEmptyEntries);

            foreach (var policy in policyParts)
            {
                if (permissions.Contains(policy.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    context.Succeed(requirement);
                    break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
