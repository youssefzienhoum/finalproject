namespace SmartWarehouse.Application.Features.Products;

public record ProductDto(
    int Id, 
    string SKU, 
    string Name, 
    string? Description, 
    int CategoryId, 
    string CategoryName,
    decimal Price, 
    int MinimumStockLevel, 
    bool IsActive);
