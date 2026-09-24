using SmartWarehouse.Domain.Common;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// External customer who places sales orders.
/// </summary>
public class Customer : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
}
