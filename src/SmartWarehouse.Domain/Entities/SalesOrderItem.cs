using SmartWarehouse.Domain.Common;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Line item in a sales order.
/// </summary>
public class SalesOrderItem : BaseEntity<int>
{
    public int SalesOrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation
    public SalesOrder SalesOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
