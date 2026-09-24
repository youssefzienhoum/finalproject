using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Application.Common.Models;
using SmartWarehouse.Application.Features.Products;

namespace SmartWarehouse.Application.Features.Products.Queries;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products.Include(p => p.Category).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(p => p.Name.Contains(request.Search) || p.SKU.Contains(request.Search));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            bool isDesc = request.SortDirection?.ToLower() == "desc";
            query = request.SortBy.ToLower() switch
            {
                "name" => isDesc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "sku" => isDesc ? query.OrderByDescending(p => p.SKU) : query.OrderBy(p => p.SKU),
                "price" => isDesc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                _ => query.OrderBy(p => p.Id)
            };
        }
        else
        {
            query = query.OrderBy(p => p.Id);
        }

        var dtoQuery = query.Select(p => new ProductDto(
            p.Id, 
            p.SKU, 
            p.Name, 
            p.Description, 
            p.CategoryId, 
            p.Category.Name, 
            p.Price, 
            p.MinimumStockLevel, 
            p.IsActive));

        return await PaginatedList<ProductDto>.CreateAsync(dtoQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}
