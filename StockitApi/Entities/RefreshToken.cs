namespace StockitApi.Entities;

public class RefreshToken : BaseEntity<Guid>
{
    public int UserId { get; set; }
    required public string TokenHash { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    required public string CreatedByIp { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceName { get; set; }

    public bool IsRevoked => RevokedAt.HasValue;

    public User? User { get; set; }
}
