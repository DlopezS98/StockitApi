using System.ComponentModel.DataAnnotations;

namespace StockitApi.Dtos;

public record RefreshTokenRequest([Required] string RefreshToken);
public record RefreshTokenResult(string RawToken, string HashedToken, DateTime ExpiresAtUtc);

public record SessionResponse(
    Guid Id,
    string IpAddress,
    string? UserAgent,
    string? DeviceName,
    DateTime CreatedAt,
    DateTime ExpiresAt
);