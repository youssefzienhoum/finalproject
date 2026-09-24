using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Application.Features.Products;

namespace SmartWarehouse.Application.Features.Products.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product == null)
        {
            throw new NotFoundException(nameof(Product), request.Id);
        }

        var category = await _context.Categories.FindAsync(new object[] { request.CategoryId }, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.SKU == request.SKU && p.Id != request.Id, cancellationToken);
        if (existingProduct != null)
        {
            throw new BusinessRuleException("SKU must be unique.");
        }

        product.SKU = request.SKU;
        product.Name = request.Name;
        product.Description = request.Description;
        product.CategoryId = request.CategoryId;
        product.Price = request.Price;
        product.MinimumStockLevel = request.MinimumStockLevel;
        product.IsActive = request.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

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
