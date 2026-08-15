using StockitApi.Entities;

namespace StockitApi.Specifications;

public class UserWithPermissionsSpecification : BaseSpecification<User>
{
    private const string PermissionsIncludePath = "Role.RolePermissions.Permission";

    public UserWithPermissionsSpecification(int id)
    {
        ApplyCriteria(u => u.Id == id);
        AddInclude(PermissionsIncludePath);
    }

    public UserWithPermissionsSpecification(string email)
    {
        ApplyCriteria(u => u.Email == email);
        AddInclude(PermissionsIncludePath);
    }
}
