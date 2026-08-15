using System.ComponentModel.DataAnnotations;

namespace StockitApi.Entities;

public abstract class BaseEntity<TKey>
{
    [Key]
    public TKey Id { get; set; } = default!;
}
