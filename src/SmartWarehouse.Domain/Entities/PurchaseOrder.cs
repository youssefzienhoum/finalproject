using SmartWarehouse.Domain.Common;
using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// A purchase order placed with a supplier to replenish inventory.
/// Follows a strict lifecycle: Draft → Submitted → Approved → Received.
/// Receiving a PO increases Inventory.Quantity and creates InventoryMovements within a transaction.
/// TotalAmount is calculated from items to avoid data inconsistency.
/// </summary>
public class PurchaseOrder : BaseEntity<int>
{
    public string OrderNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public int WarehouseId { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }

    // Navigation
    public Supplier Supplier { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}
