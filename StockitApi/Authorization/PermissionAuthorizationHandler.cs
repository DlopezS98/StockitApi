using Microsoft.AspNetCore.Authorization;

namespace StockitApi.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        bool hasPermission = context.User.Claims.Any(c =>
            c.Type == PermissionAuthorization.PermissionClaimType &&
            string.Equals(c.Value, requirement.Permission, StringComparison.OrdinalIgnoreCase));

        if (hasPermission) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
