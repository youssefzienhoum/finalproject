using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.Reports.Queries;

public class GetLowStockReportQueryHandler : IRequestHandler<GetLowStockReportQuery, PaginatedList<LowStockReportDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLowStockReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<LowStockReportDto>> Handle(GetLowStockReportQuery request, CancellationToken cancellationToken)
    {
        var productQuery = _context.Products.AsNoTracking().Where(p => p.IsActive);

        if (request.CategoryId.HasValue)
        {
            productQuery = productQuery.Where(p => p.CategoryId == request.CategoryId);
        }

        var products = await productQuery.ToListAsync(cancellationToken);
        
        var inventories = await _context.Inventories
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var lowStockList = new List<LowStockReportDto>();

        foreach (var p in products)
        {
            var productInventories = inventories.Where(i => i.ProductId == p.Id);
            int totalAvailable = productInventories.Sum(i => i.Quantity - i.ReservedQuantity);

            if (totalAvailable <= p.MinimumStockLevel)
            {
                lowStockList.Add(new LowStockReportDto(
                    p.Id,
                    p.Name,
                    p.SKU,
                    p.MinimumStockLevel,
                    totalAvailable
                ));
            }
        }

        // Apply pagination in memory since the group by/having is complex in EF Core with client side evaluation.
        // For a production app with millions of products, this would be a custom SQL query or view.
        var pagedItems = lowStockList
            .OrderBy(x => x.ProductName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PaginatedList<LowStockReportDto>(pagedItems, lowStockList.Count, request.PageNumber, request.PageSize);
    }
}
