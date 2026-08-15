using StockitApi.Dtos;
using StockitApi.Entities;
using StockitApi.Interfaces;
using StockitApi.Specifications;

namespace StockitApi.Services;

public class UserService(
    IUnitOfWork unitOfWork,
    IPasswordHasherService passwordHasher,
    TimeProvider timeProvider) : IUserService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasherService _passwordHasher = passwordHasher;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<UserResponse?> GetByIdAsync(int id)
    {
        User? user = await _unitOfWork.Users.GetFirstAsync(new UserWithRoleSpecification(id));
        return user is null ? null : ToResponse(user);
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync()
    {
        IReadOnlyList<User> users = await _unitOfWork.Users.ListAsync(new UserWithRoleSpecification());
        return [.. users.Select(ToResponse)];
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request)
    {
        User user = await _unitOfWork.Users.GetFirstAsync(new UserWithRoleSpecification(id)) ?? throw new KeyNotFoundException("User not found.");
        Role role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId) ?? throw new InvalidOperationException("Role not found.");

        user.FullName = $"{request.FirstName} {request.LastName}";
        user.RoleId = role.Id;
        user.Role = role;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return ToResponse(user);
    }

    public async Task DeactivateAsync(int id)
    {
        User user = await _unitOfWork.Users.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found.");
        user.IsActive = false;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(int id, ChangePasswordRequest request)
    {
        User user = await _unitOfWork.Users.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found.");

        bool isCurrentPasswordValid = _passwordHasher.VerifyPassword(
            user.PasswordHash,
            request.CurrentPassword,
            Convert.FromBase64String(user.Salt)
        );
        if (!isCurrentPasswordValid) throw new UnauthorizedAccessException("Current password is incorrect.");

        (string hashedPassword, byte[] salt) = _passwordHasher.HashPassword(request.NewPassword);
        user.PasswordHash = hashedPassword;
        user.Salt = Convert.ToBase64String(salt);

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    private static UserResponse ToResponse(User user)
    {
        return new UserResponse(
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.RoleId,
            user.Role?.Name ?? string.Empty,
            user.CreatedAt
        );
    }
}
