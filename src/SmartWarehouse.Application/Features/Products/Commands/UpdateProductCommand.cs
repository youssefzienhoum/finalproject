using MediatR;
using SmartWarehouse.Application.Features.Products;

namespace SmartWarehouse.Application.Features.Products.Commands;

public record UpdateProductCommand(
    int Id,
    string SKU, 
    string Name, 
    string? Description, 
    int CategoryId, 
    decimal Price, 
    int MinimumStockLevel, 
    bool IsActive) : IRequest<ProductDto>;
