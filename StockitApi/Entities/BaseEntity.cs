using System.ComponentModel.DataAnnotations;

namespace StockitApi.Entities;

public abstract class BaseEntity<TKey>
{
    [Key]
    required public TKey Id { get; set; }
}
