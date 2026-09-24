using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.Reports.Queries;

public class GetInventoryReportQueryHandler : IRequestHandler<GetInventoryReportQuery, PaginatedList<InventoryReportDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<InventoryReportDto>> Handle(GetInventoryReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .AsNoTracking()
            .AsQueryable();

        if (request.WarehouseId.HasValue)
            query = query.Where(i => i.WarehouseId == request.WarehouseId);
            
        if (request.ProductId.HasValue)
            query = query.Where(i => i.ProductId == request.ProductId);

        if (request.CategoryId.HasValue)
            query = query.Where(i => i.Product.CategoryId == request.CategoryId);

        var projected = query
            .OrderBy(i => i.Product.Name)
            .Select(i => new InventoryReportDto(
                i.ProductId,
                i.Product.Name,
                i.Product.SKU,
                i.WarehouseId,
                i.Warehouse.Name,
                i.Quantity,
                i.ReservedQuantity,
                i.Quantity - i.ReservedQuantity
            ));

        return await PaginatedList<InventoryReportDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
