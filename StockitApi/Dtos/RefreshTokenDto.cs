namespace StockitApi.Dtos;

public record RefreshTokeRequest(string RefreshToken);
public record RefreshTokenResult(string RawToken, string HashedToken, DateTime ExpiresAtUtc);