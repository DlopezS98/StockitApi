namespace StockitApi.Dtos;

public record RefreshTokenResult(string RawToken, string HashedToken, DateTime ExpiresAtUtc);