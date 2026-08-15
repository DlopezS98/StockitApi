using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace StockitApi.Authorization;

// Dynamically builds an authorization policy per permission code instead of registering one upfront per permission.
public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider = new(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PermissionAuthorization.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            string permission = policyName[PermissionAuthorization.PolicyPrefix.Length..];
            AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}
