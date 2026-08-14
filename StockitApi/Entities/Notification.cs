namespace StockitApi.Entities;

public class Notification : BaseEntity<Guid>
{
    public int UserId { get; set; }
    required public string Title { get; set; }
    required public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
