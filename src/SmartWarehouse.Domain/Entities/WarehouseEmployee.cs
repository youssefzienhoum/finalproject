namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Many-to-many join entity between Warehouse and User (employee).
/// Tracks which employees are assigned to which warehouses with an assignment date.
/// An employee cannot be assigned to the same warehouse twice.
/// </summary>
public class WarehouseEmployee
{
    public int WarehouseId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Warehouse Warehouse { get; set; } = null!;
    public User User { get; set; } = null!;
}
