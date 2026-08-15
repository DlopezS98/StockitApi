using StockitApi.Dtos;

namespace StockitApi.Interfaces;

public interface IUserService
{
    Task<UserResponse?> GetByIdAsync(int id);
    Task<IReadOnlyList<UserResponse>> GetAllAsync();
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request);
    Task DeactivateAsync(int id);
    Task ChangePasswordAsync(int id, ChangePasswordRequest request);
}
