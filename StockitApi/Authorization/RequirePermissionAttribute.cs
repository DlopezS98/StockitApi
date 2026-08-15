using Microsoft.AspNetCore.Authorization;

namespace StockitApi.Authorization;

// Usage: [RequirePermission("products.read")] - also enforces authentication like [Authorize].
public class RequirePermissionAttribute(string permission) : AuthorizeAttribute(PermissionAuthorization.GetPolicyName(permission))
{
}
