using SmartWarehouse.Domain.Common;
using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Immutable audit record of every physical stock change.
/// Every time Inventory.Quantity changes, a corresponding InventoryMovement is created.
/// This provides a complete audit trail and enables stock movement reporting.
/// ReferenceId links back to the source document (PurchaseOrder, SalesOrder, StockTransfer).
/// </summary>
public class InventoryMovement : BaseEntity<long>
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
    public MovementType MovementType { get; set; }
    public string? ReferenceId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
}
