namespace StockitApi.Entities;

public class Permission : BaseEntity<int>
{
    required public string Code { get; set; }
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
