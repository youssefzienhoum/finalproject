using SmartWarehouse.Domain.Common;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// A physical warehouse location where products are stored.
/// Warehouses have employees assigned to them and maintain per-product inventory records.
/// Inactive warehouses cannot receive new stock.
/// </summary>
public class Warehouse : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<WarehouseEmployee> Employees { get; set; } = new List<WarehouseEmployee>();
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}
