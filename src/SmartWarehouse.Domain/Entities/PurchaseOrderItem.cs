using SmartWarehouse.Domain.Common;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Line item in a purchase order specifying product, quantity, and unit price.
/// TotalPrice = Quantity * UnitPrice (stored for reporting efficiency).
/// </summary>
public class PurchaseOrderItem : BaseEntity<int>
{
    public int PurchaseOrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
