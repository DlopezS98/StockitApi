namespace StockitApi.Entities;

public class User : BaseEntity<int>
{
    required public string Username { get; set; }
    required public string Email { get; set; }
    required public string PasswordHash { get; set; }
    required public string FullName { get; set; }
    public int RoleId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public Role? Role { get; set; }
    public User? CreatedByUser { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<StockMovement> StockMovements { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
    public ICollection<AuditLog> AuditLogs { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<ApiUsageLog> ApiUsageLogs { get; set; } = [];
}
