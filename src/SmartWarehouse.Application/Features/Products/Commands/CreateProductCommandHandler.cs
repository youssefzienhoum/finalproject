using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Application.Features.Products;

namespace SmartWarehouse.Application.Features.Products.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync(new object[] { request.CategoryId }, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.SKU == request.SKU, cancellationToken);
        if (existingProduct != null)
        {
            throw new BusinessRuleException("SKU must be unique.");
        }

        var product = new Product
        {
            SKU = request.SKU,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            Price = request.Price,
            MinimumStockLevel = request.MinimumStockLevel,
            IsActive = request.IsActive
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return new ProductDto(
            product.Id, 
            product.SKU, 
            product.Name, 
            product.Description, 
            product.CategoryId, 
            category.Name, 
            product.Price, 
            product.MinimumStockLevel, 
            product.IsActive);
    }
}
