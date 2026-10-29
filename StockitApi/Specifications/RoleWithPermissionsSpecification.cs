using StockitApi.Entities;

namespace StockitApi.Specifications;

public class RoleWithPermissionsSpecification : BaseSpecification<Role>
{
    private const string PermissionsIncludePath = "RolePermissions.Permission";

    public RoleWithPermissionsSpecification()
    {
        AddInclude(PermissionsIncludePath);
        ApplyOrderBy(r => r.Name);
    }

    public RoleWithPermissionsSpecification(int id)
    {
        ApplyCriteria(r => r.Id == id);
        AddInclude(PermissionsIncludePath);
    }
}
