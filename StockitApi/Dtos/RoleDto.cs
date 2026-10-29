using System.ComponentModel.DataAnnotations;

namespace StockitApi.Dtos;

public record RoleResponse(
    int Id,
    string Name,
    string? Description,
    IReadOnlyList<string> Permissions
);

public record PermissionResponse(
    int Id,
    string Code,
    string? Description
);

public record CreateRoleRequest(
    [Required, MaxLength(50)] string Name,
    [MaxLength(200)] string? Description,
    [Required] IReadOnlyList<string> Permissions
);

public record UpdateRoleRequest(
    [Required, MaxLength(50)] string Name,
    [MaxLength(200)] string? Description,
    [Required] IReadOnlyList<string> Permissions
);
