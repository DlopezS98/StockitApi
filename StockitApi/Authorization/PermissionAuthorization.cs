namespace StockitApi.Authorization;

public static class PermissionAuthorization
{
    public const string PermissionClaimType = "Permissions";
    public const string PolicyPrefix = "Permission:";

    public static string GetPolicyName(string permission) => $"{PolicyPrefix}{permission}";
}