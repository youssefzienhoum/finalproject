using SmartWarehouse.Domain.Common;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Product represents a physical item tracked in the warehouse system.
/// Products have a unique SKU, belong to a category, and define a minimum stock level
/// for low-stock detection. A product must be active before it can be used in any
/// inventory operation (purchase, sale, transfer).
/// </summary>
public class Product : BaseEntity<int>
{
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public int MinimumStockLevel { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Category Category { get; set; } = null!;
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
    public ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();
}
