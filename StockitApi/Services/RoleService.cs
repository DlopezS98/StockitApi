using StockitApi.Dtos;
using StockitApi.Entities;
using StockitApi.Interfaces;
using StockitApi.Specifications;
using RoleDefinitions = StockitApi.Models.Role;

namespace StockitApi.Services;

public class RoleService(IUnitOfWork unitOfWork) : IRoleService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<RoleResponse?> GetByIdAsync(int id)
    {
        Role? role = await _unitOfWork.Roles.GetFirstAsync(new RoleWithPermissionsSpecification(id));
        return role is null ? null : ToResponse(role);
    }

    public async Task<IReadOnlyList<RoleResponse>> GetAllAsync()
    {
        IReadOnlyList<Role> roles = await _unitOfWork.Roles.ListAsync(new RoleWithPermissionsSpecification());
        return [.. roles.Select(ToResponse)];
    }

    public async Task<IReadOnlyList<PermissionResponse>> GetAllPermissionsAsync()
    {
        IReadOnlyList<Permission> permissions = await _unitOfWork.Permissions.GetAllAsync();
        return [.. permissions
            .OrderBy(p => p.Code)
            .Select(p => new PermissionResponse(p.Id, p.Code, p.Description))];
    }

    public async Task<RoleResponse> CreateAsync(CreateRoleRequest request)
    {
        string name = request.Name.Trim();
        await EnsureNameIsAvailableAsync(name);
        IReadOnlyList<Permission> permissions = await ResolvePermissionsAsync(request.Permissions);

        Role role = new()
        {
            Name = name,
            Description = request.Description,
            RolePermissions = [.. permissions.Select(p => new RolePermission { PermissionId = p.Id, Permission = p })],
        };

        await _unitOfWork.Roles.AddAsync(role);
        await _unitOfWork.SaveChangesAsync();

        return ToResponse(role);
    }

    public async Task<RoleResponse> UpdateAsync(int id, UpdateRoleRequest request)
    {
        Role role = await _unitOfWork.Roles.GetFirstAsync(new RoleWithPermissionsSpecification(id)) ?? throw new KeyNotFoundException("Role not found.");
        EnsureNotBuiltIn(role);

        string name = request.Name.Trim();
        await EnsureNameIsAvailableAsync(name, role.Id);
        IReadOnlyList<Permission> permissions = await ResolvePermissionsAsync(request.Permissions);

        role.Name = name;
        role.Description = request.Description;

        // The role is tracked, so diff the join rows in place instead of calling Update on the whole graph.
        HashSet<int> requestedIds = [.. permissions.Select(p => p.Id)];
        List<RolePermission> removed = [.. role.RolePermissions.Where(rp => !requestedIds.Contains(rp.PermissionId))];
        _unitOfWork.RemoveRolePermissions(removed);
        foreach (RolePermission rolePermission in removed) role.RolePermissions.Remove(rolePermission);

        HashSet<int> existingIds = [.. role.RolePermissions.Select(rp => rp.PermissionId)];
        foreach (Permission permission in permissions.Where(p => !existingIds.Contains(p.Id)))
        {
            role.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id, Permission = permission });
        }

        await _unitOfWork.SaveChangesAsync();

        return ToResponse(role);
    }

    public async Task DeleteAsync(int id)
    {
        Role role = await _unitOfWork.Roles.GetFirstAsync(new RoleWithPermissionsSpecification(id)) ?? throw new KeyNotFoundException("Role not found.");
        EnsureNotBuiltIn(role);

        User? assignedUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.RoleId == role.Id);
        if (assignedUser is not null) throw new InvalidOperationException("Cannot delete a role that is assigned to users.");

        // RolePermission uses a restrict delete, so the join rows have to be removed explicitly.
        _unitOfWork.RemoveRolePermissions(role.RolePermissions);
        await _unitOfWork.Roles.DeleteAsync(role);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task EnsureNameIsAvailableAsync(string name, int? excludedRoleId = null)
    {
        Role? existing = await _unitOfWork.Roles.FirstOrDefaultAsync(r => r.Name == name && r.Id != excludedRoleId);
        if (existing is not null) throw new InvalidOperationException("A role with this name already exists.");
    }

    // Built-in roles are re-seeded on startup, so edits to them would be overwritten or duplicated.
    private static void EnsureNotBuiltIn(Role role)
    {
        if (RoleDefinitions.BuiltInRoles.Contains(role.Name, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Built-in roles cannot be modified or deleted.");
    }

    private async Task<IReadOnlyList<Permission>> ResolvePermissionsAsync(IReadOnlyList<string> codes)
    {
        List<string> requestedCodes = [.. codes.Select(c => c.Trim()).Distinct(StringComparer.OrdinalIgnoreCase)];
        IReadOnlyList<Permission> permissions = await _unitOfWork.Permissions.GetAsync(p => requestedCodes.Contains(p.Code));

        List<string> unknownCodes = [.. requestedCodes.Where(c => !permissions.Any(p => string.Equals(p.Code, c, StringComparison.OrdinalIgnoreCase)))];
        if (unknownCodes.Count > 0) throw new InvalidOperationException($"Unknown permissions: {string.Join(", ", unknownCodes)}.");

        return permissions;
    }

    private static RoleResponse ToResponse(Role role)
    {
        return new RoleResponse(
            role.Id,
            role.Name,
            role.Description,
            [.. role.RolePermissions.Select(rp => rp.Permission?.Code ?? string.Empty).Order()]
        );
    }
}
