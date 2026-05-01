using BaseAPI.Middleware.JWTMidlleware;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
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

        if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
