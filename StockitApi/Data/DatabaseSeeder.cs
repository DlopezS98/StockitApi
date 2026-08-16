using System.ComponentModel;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StockitApi.Entities;
using StockitApi.Interfaces;
using PermissionCodes = StockitApi.Models.Permission;
using RoleDefinitions = StockitApi.Models.Role;

namespace StockitApi.Data;

// Seeds baseline roles, permissions, role-permission mappings, and a default admin user.
public static class DatabaseSeeder
{
    private const string AdminRoleName = RoleDefinitions.Admin;
    private const string DefaultAdminEmail = "danny.lopez@stockit.com";
    private const string DefaultAdminPassword = "Tester017!";

    private static readonly (string Name, string? Description)[] Roles =
    [
        (RoleDefinitions.Admin, "Full system access"),
        (RoleDefinitions.Manager, "Products, inventory, invoices, analytics"),
        (RoleDefinitions.SalesRep, "Sell products, create invoices"),
        (RoleDefinitions.WarehouseStaff, "Manage stock"),
        (RoleDefinitions.Auditor, "Read-only oversight"),
    ];

    // Codes and descriptions are sourced from the Permission class so both stay in sync.
    private static readonly (string Code, string? Description)[] Permissions = GetPermissionDefinitions();

    // Sourced from the Role class so seeding stays in sync with the role-permission mappings.
    private static readonly Dictionary<string, string[]> RolePermissionCodes = new()
    {
        [RoleDefinitions.Admin] = RoleDefinitions.AdminPermissions,
        [RoleDefinitions.Manager] = RoleDefinitions.ManagerPermissions,
        [RoleDefinitions.SalesRep] = RoleDefinitions.SalesRepPermissions,
        [RoleDefinitions.WarehouseStaff] = RoleDefinitions.WarehouseStaffPermissions,
        [RoleDefinitions.Auditor] = RoleDefinitions.AuditorPermissions,
    };

    // Reflects over the Permission class' nested types to collect every code/description constant.
    private static (string Code, string? Description)[] GetPermissionDefinitions()
    {
        return [.. typeof(PermissionCodes)
            .GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static))
            .Where(field => field.IsLiteral && field.FieldType == typeof(string))
            .Select(field => (
                Code: (string)field.GetRawConstantValue()!,
                Description: field.GetCustomAttribute<DescriptionAttribute>()?.Description))];
    }

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
