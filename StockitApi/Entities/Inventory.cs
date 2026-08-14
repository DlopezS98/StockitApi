namespace StockitApi.Entities;

public class Inventory : BaseEntity<int>
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }

    public Product? Product { get; set; }
    public Warehouse? Warehouse { get; set; }
}
