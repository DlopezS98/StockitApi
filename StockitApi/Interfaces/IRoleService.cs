using StockitApi.Dtos;

namespace StockitApi.Interfaces;

public interface IRoleService
{
    Task<RoleResponse?> GetByIdAsync(int id);
    Task<IReadOnlyList<RoleResponse>> GetAllAsync();
    Task<IReadOnlyList<PermissionResponse>> GetAllPermissionsAsync();
    Task<RoleResponse> CreateAsync(CreateRoleRequest request);
    Task<RoleResponse> UpdateAsync(int id, UpdateRoleRequest request);
    Task DeleteAsync(int id);
}
