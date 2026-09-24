using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.Inventory.Queries;

public class GetInventoryQueryHandler : IRequestHandler<GetInventoryQuery, PaginatedList<InventoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<InventoryDto>> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .AsNoTracking()
            .AsQueryable();

        if (request.ProductId.HasValue)
        {
            query = query.Where(i => i.ProductId == request.ProductId.Value);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(i => i.WarehouseId == request.WarehouseId.Value);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(i => i.Product.CategoryId == request.CategoryId.Value);
        }

        if (request.LowStock.HasValue && request.LowStock.Value)
        {
            query = query.Where(i => (i.Quantity - i.ReservedQuantity) <= i.Product.MinimumStockLevel);
        }

        query = query.OrderBy(i => i.WarehouseId).ThenBy(i => i.ProductId);

        var dtoQuery = query.Select(i => new InventoryDto(
            i.ProductId,
            i.Product.Name,
            i.Product.SKU,
            i.WarehouseId,
            i.Warehouse.Name,
            i.Quantity,
            i.ReservedQuantity,
            i.Quantity - i.ReservedQuantity // Calculate AvailableQuantity
        ));

        return await PaginatedList<InventoryDto>.CreateAsync(dtoQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}
