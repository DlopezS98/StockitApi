namespace StockitApi.Entities;

public class AuditLog : BaseEntity<Guid>
{
    public int UserId { get; set; }
    required public string Action { get; set; }
    required public string EntityType { get; set; }
    public int EntityId { get; set; }

    // JSON representation of changes
    public string? Changes { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
