namespace StockitApi.Entities;

public class Role : BaseEntity<int>
{
    required public string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<User> Users { get; set; } = [];
}
