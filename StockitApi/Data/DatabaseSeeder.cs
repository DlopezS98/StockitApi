using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockitApi.Entities;
using StockitApi.Interfaces;

namespace StockitApi.Data;

// Seeds baseline roles, permissions, role-permission mappings, and a default admin user.
public static class DatabaseSeeder
{
    private const string AdminRoleName = "Admin";
    private const string DefaultAdminEmail = "danny.lopez@stockit.com";
    private const string DefaultAdminPassword = "Tester017!";

    private static readonly (string Name, string? Description)[] Roles =
    [
        ("Admin", "Full system access"),
        ("Manager", "Products, inventory, invoices, analytics"),
        ("SalesRep", "Sell products, create invoices"),
        ("WarehouseStaff", "Manage stock"),
        ("Auditor", "Read-only oversight"),
    ];

    private static readonly (string Code, string? Description)[] Permissions =
    [
        ("users.read", "View users"),
        ("users.write", "Create/edit users"),
        ("users.delete", "Deactivate users"),
        ("roles.manage", "Manage roles/permissions"),
        ("products.read", "View products"),
        ("products.write", "Create/edit products"),
        ("inventory.read", "View stock levels"),
        ("inventory.adjust", "Adjust stock"),
        ("customers.read", "View customers"),
        ("customers.write", "Create/edit customers"),
        ("invoices.create", "Create/issue invoices"),
        ("invoices.read", "View invoices"),
        ("invoices.update", "Update invoices"),
        ("invoices.void", "Void invoices"),
        ("invoices.export", "Export invoices"),
        ("analytics.read", "View analytics"),
        ("logs.read", "View audit logs"),
        ("metrics.read", "View usage metrics"),
    ];

    // Admin implicitly gets every permission and is not listed here.
    private static readonly Dictionary<string, string[]> RolePermissionCodes = new()
    {
        ["Manager"] =
        [
            "users.read", "products.read", "products.write", "inventory.read", "inventory.adjust",
            "customers.read", "customers.write", "invoices.create", "invoices.read", "invoices.void",
            "invoices.export", "analytics.read", "metrics.read",
        ],
        ["SalesRep"] =
        [
            "products.read", "inventory.read", "customers.read", "customers.write",
            "invoices.create", "invoices.read",
        ],
        ["WarehouseStaff"] =
        [
            "products.read", "inventory.read", "inventory.adjust", "invoices.read",
        ],
        ["Auditor"] =
        [
            "users.read", "products.read", "inventory.read", "customers.read", "invoices.read",
            "invoices.export", "analytics.read", "logs.read", "metrics.read",
        ],
    };

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        StockitContext context = serviceProvider.GetRequiredService<StockitContext>();
        IPasswordHasherService passwordHasher = serviceProvider.GetRequiredService<IPasswordHasherService>();
        TimeProvider timeProvider = serviceProvider.GetRequiredService<TimeProvider>();

        await SeedRolesAsync(context);
        await SeedPermissionsAsync(context);
        await SeedRolePermissionsAsync(context);
        await SeedAdminUserAsync(context, passwordHasher, timeProvider);
    }

    private static async Task SeedRolesAsync(StockitContext context)
    {
        foreach ((string name, string? description) in Roles)
        {
            bool exists = await context.Roles.AnyAsync(r => r.Name == name);
            if (exists) continue;

            context.Roles.Add(new Role { Name = name, Description = description });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedPermissionsAsync(StockitContext context)
    {
        foreach ((string code, string? description) in Permissions)
        {
            bool exists = await context.Permissions.AnyAsync(p => p.Code == code);
            if (exists) continue;

            context.Permissions.Add(new Permission { Code = code, Description = description });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolePermissionsAsync(StockitContext context)
    {
        List<Role> roles = await context.Roles.ToListAsync();
        List<Permission> permissions = await context.Permissions.ToListAsync();

        Role adminRole = roles.First(r => r.Name == AdminRoleName);
        await AddMissingRolePermissionsAsync(context, permissions, adminRole, [.. permissions.Select(p => p.Code)]);

        foreach ((string roleName, string[] codes) in RolePermissionCodes)
        {
            Role role = roles.First(r => r.Name == roleName);
            await AddMissingRolePermissionsAsync(context, permissions, role, codes);
        }
    }

    private static async Task AddMissingRolePermissionsAsync(
        StockitContext context, List<Permission> permissions, Role role, string[] permissionCodes)
    {
        List<int> permissionIds = [.. permissions.Where(p => permissionCodes.Contains(p.Code)).Select(p => p.Id)];
        List<int> existingPermissionIds = await context.RolePermissions
            .Where(rp => rp.RoleId == role.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        foreach (int permissionId in permissionIds.Except(existingPermissionIds))
        {
            context.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permissionId });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(StockitContext context, IPasswordHasherService passwordHasher, TimeProvider timeProvider)
    {
        bool exists = await context.Users.AnyAsync(u => u.Email == DefaultAdminEmail);
        if (exists) return;

        Role adminRole = await context.Roles.FirstAsync(r => r.Name == AdminRoleName);
        (string hashedPassword, byte[] salt) = passwordHasher.HashPassword(DefaultAdminPassword);

        context.Users.Add(new User
        {
            Username = DefaultAdminEmail,
            Email = DefaultAdminEmail,
            PasswordHash = hashedPassword,
            Salt = Convert.ToBase64String(salt),
            FullName = "System Admin",
            RoleId = adminRole.Id,
            IsActive = true,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        });

        await context.SaveChangesAsync();
    }
}
