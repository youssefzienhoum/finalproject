using SmartWarehouse.Domain.Common;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Product category for organizing products.
/// A category groups related products (e.g., "Electronics", "Raw Materials").
/// Cannot be deleted if products are associated with it.
/// </summary>
public class Category : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
