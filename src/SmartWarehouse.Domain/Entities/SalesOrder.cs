using SmartWarehouse.Domain.Common;
using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// A sales order from a customer that will ship products from a warehouse.
/// Follows lifecycle: Draft → Confirmed → Shipped → Completed.
/// 
/// Confirming reserves inventory (increases ReservedQuantity).
/// Shipping decreases both Quantity and ReservedQuantity, creating InventoryMovements.
/// Cancellation releases any reservations.
/// </summary>
public class SalesOrder : BaseEntity<int>
{
    public string OrderNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int WarehouseId { get; set; }
    public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public Customer Customer { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();
}
