namespace StockitApi.Authorization;

public static class PermissionAuthorization
{
    public const string PermissionClaimType = "Permission";
    public const string PolicyPrefix = "Permission:";

    public static string GetPolicyName(string permission) => $"{PolicyPrefix}{permission}";
}