namespace StockitApi.Entities;

public class ApiUsageLog : BaseEntity<Guid>
{
    public int? UserId { get; set; }
    required public string Endpoint { get; set; }
    required public string HttpMethod { get; set; }
    public int StatusCode { get; set; }
    required public string IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int ResponseTimeMs { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
