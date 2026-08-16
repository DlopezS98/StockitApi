using StockitApi.Dtos;

namespace StockitApi.Interfaces;

public interface IAuthenticationService
{
    Task<TokenResponse> AuthenticateAsync(AuthRequest request);
    Task<UserResponse> RegisterAsync(RegisterRequest request);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
    Task<IReadOnlyList<SessionResponse>> GetActiveSessionsAsync(int userId);
    Task RevokeSessionAsync(int userId, Guid sessionId);
    Task RevokeAllSessionsAsync(int userId, string? currentRefreshToken = null);
}