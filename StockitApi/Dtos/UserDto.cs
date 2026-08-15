namespace StockitApi.Dtos;

public record UserResponse(
    int Id,
    string Username,
    string Email,
    string FullName,
    int RoleId,
    string RoleName,
    DateTime CreatedAt
);