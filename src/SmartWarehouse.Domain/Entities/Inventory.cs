using SmartWarehouse.Domain.Common;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Represents the quantity of a specific product in a specific warehouse.
/// Uses a composite key of (ProductId, WarehouseId).
/// 
/// Quantity = total physical stock on hand.
/// ReservedQuantity = stock reserved by confirmed (but not yet shipped) sales orders.
/// AvailableQuantity = Quantity - ReservedQuantity (calculated, not stored).
/// 
/// RowVersion is critical here — it prevents two concurrent operations from
/// overselling the same inventory (optimistic concurrency).
/// </summary>
public class Inventory
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// AvailableQuantity is calculated, not stored.
    /// This avoids data inconsistency between Quantity, ReservedQuantity, and AvailableQuantity.
    /// </summary>
    public int AvailableQuantity => Quantity - ReservedQuantity;

    /// <summary>
    /// Optimistic concurrency token for preventing race conditions on stock operations.
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    // Navigation
    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
}
