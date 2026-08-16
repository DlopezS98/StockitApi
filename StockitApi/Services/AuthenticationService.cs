
using StockitApi.Dtos;
using StockitApi.Entities;
using StockitApi.Interfaces;
using StockitApi.Specifications;

namespace StockitApi.Services;

public class AuthenticationService(
    IUnitOfWork unitOfWork,
    IPasswordHasherService passwordHasher,
    ITokenService tokenService,
    IAppRequestContext appRequestContext,
    TimeProvider timeProvider) : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasherService _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IAppRequestContext _appRequestContext = appRequestContext;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<TokenResponse> AuthenticateAsync(AuthRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            throw new ArgumentException("Email and password must be provided.");

        User? user = await _unitOfWork.Users.GetFirstAsync(new UserWithPermissionsSpecification(request.Email));
        bool verifyPassword(User _user) => _passwordHasher.VerifyPassword(
            _user.PasswordHash,
            request.Password,
            Convert.FromBase64String(_user.Salt)
        );
        if (user is null || !verifyPassword(user)) throw new UnauthorizedAccessException("Invalid email or password.");

        RefreshTokenResult refreshTokenResult = _tokenService.GenerateRefreshToken();
        RefreshToken refreshToken = new()
        {
            Id = Guid.NewGuid(),
            TokenHash = refreshTokenResult.HashedToken,
            UserId = user.Id,
            CreatedByIp = _appRequestContext.IpAddress,
            UserAgent = _appRequestContext.UserAgent,
            DeviceName = _appRequestContext.DeviceName,
            ExpiresAt = refreshTokenResult.ExpiresAtUtc,
            CreatedAt = _timeProvider.GetUtcNow().UtcDateTime
        };
        (string token, DateTime expires) = _tokenService.GenerateToken(user, refreshToken.Id);
        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new TokenResponse(token, refreshTokenResult.RawToken, expires, refreshTokenResult.ExpiresAtUtc);
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        string hashedToken = _tokenService.ComputeHash(refreshToken);
        RefreshToken? refreshTokenEntity = await _unitOfWork.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == hashedToken);
        if (refreshTokenEntity is null) throw new UnauthorizedAccessException("Invalid refresh token.");
        if (refreshTokenEntity.IsRevoked || refreshTokenEntity.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
            throw new UnauthorizedAccessException("Refresh token is expired or revoked.");

        User? user = await _unitOfWork.Users.GetFirstAsync(new UserWithPermissionsSpecification(refreshTokenEntity.UserId));
        if (user is null) throw new UnauthorizedAccessException("User not found.");

        RefreshTokenResult refreshTokenResult = _tokenService.GenerateRefreshToken();
        RefreshToken newRefreshToken = new()
        {
            Id = Guid.NewGuid(),
            TokenHash = refreshTokenResult.HashedToken,
            UserId = user.Id,
            CreatedByIp = _appRequestContext.IpAddress,
            UserAgent = _appRequestContext.UserAgent,
            DeviceName = _appRequestContext.DeviceName,
            ExpiresAt = refreshTokenResult.ExpiresAtUtc,
            CreatedAt = _timeProvider.GetUtcNow().UtcDateTime
        };
        (string token, DateTime expiresAtUtc) = _tokenService.GenerateToken(user, newRefreshToken.Id);
        refreshTokenEntity.RevokedAt = _timeProvider.GetUtcNow().UtcDateTime;
        await _unitOfWork.RefreshTokens.UpdateAsync(refreshTokenEntity);
        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new TokenResponse(token, refreshTokenResult.RawToken, expiresAtUtc, refreshTokenResult.ExpiresAtUtc);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        string hashedToken = _tokenService.ComputeHash(refreshToken);
        RefreshToken? refreshTokenEntity = await _unitOfWork.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == hashedToken);
        if (refreshTokenEntity is null || refreshTokenEntity.IsRevoked) return;

        refreshTokenEntity.RevokedAt = _timeProvider.GetUtcNow().UtcDateTime;
        await _unitOfWork.RefreshTokens.UpdateAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<SessionResponse>> GetActiveSessionsAsync(int userId)
    {
        IReadOnlyList<RefreshToken> tokens = await _unitOfWork.RefreshTokens.GetAsync(rt => rt.UserId == userId);
        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;

        return [.. tokens
            .Where(rt => !rt.IsRevoked && rt.ExpiresAt > now)
            .OrderByDescending(rt => rt.CreatedAt)
            .Select(rt => new SessionResponse(rt.Id, rt.CreatedByIp, rt.UserAgent, rt.DeviceName, rt.CreatedAt, rt.ExpiresAt))];
    }

    public async Task RevokeSessionAsync(int userId, Guid sessionId)
    {
        RefreshToken? refreshTokenEntity = await _unitOfWork.RefreshTokens.GetByIdAsync(sessionId);
        if (refreshTokenEntity is null || refreshTokenEntity.UserId != userId)
            throw new KeyNotFoundException("Session not found.");
        if (refreshTokenEntity.IsRevoked) return;

        refreshTokenEntity.RevokedAt = _timeProvider.GetUtcNow().UtcDateTime;
        await _unitOfWork.RefreshTokens.UpdateAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RevokeAllSessionsAsync(int userId, string? currentRefreshToken = null)
    {
        string? currentTokenHash = currentRefreshToken is null ? null : _tokenService.ComputeHash(currentRefreshToken);
        IReadOnlyList<RefreshToken> tokens = await _unitOfWork.RefreshTokens.GetAsync(rt => rt.UserId == userId);
        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;

        List<RefreshToken> tokensToRevoke = [.. tokens.Where(rt => !rt.IsRevoked && rt.TokenHash != currentTokenHash)];
        foreach (RefreshToken token in tokensToRevoke)
            token.RevokedAt = now;

        await _unitOfWork.RefreshTokens.UpdateAsync(tokensToRevoke);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        string email = request.Email;
        string password = request.Password;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Email and password must be provided.");

        email = email.ToLower().Trim();
        var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null) throw new InvalidOperationException("A user with this email already exists.");

        Role role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId) ?? throw new InvalidOperationException("Role not found.");

        (string hashedPassword, byte[] salt) = _passwordHasher.HashPassword(password);
        User newUser = new()
        {
            Email = email,
            Username = email,
            PasswordHash = hashedPassword,
            FullName = $"{request.FirstName} {request.LastName}",
            RoleId = role.Id,
            Salt = Convert.ToBase64String(salt),
            CreatedAt = _timeProvider.GetUtcNow().UtcDateTime,
            IsActive = true,
        };

        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();

        return new UserResponse(
            newUser.Id, 
            newUser.Username, 
            newUser.Email, 
            newUser.FullName, 
            role.Id, 
            role.Name,
            newUser.CreatedAt
        );
    }
}