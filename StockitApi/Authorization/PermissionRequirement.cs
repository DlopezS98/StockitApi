using Microsoft.AspNetCore.Authorization;

namespace StockitApi.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
