using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.Reports.Queries;

public class GetStockMovementReportQueryHandler : IRequestHandler<GetStockMovementReportQuery, PaginatedList<StockMovementReportDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStockMovementReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<StockMovementReportDto>> Handle(GetStockMovementReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.InventoryMovements
            .Include(im => im.Product)
            .Include(im => im.Warehouse)
            .AsNoTracking()
            .AsQueryable();

        if (request.WarehouseId.HasValue)
            query = query.Where(im => im.WarehouseId == request.WarehouseId);

        if (request.ProductId.HasValue)
            query = query.Where(im => im.ProductId == request.ProductId);

        if (request.MovementType.HasValue)
            query = query.Where(im => im.MovementType == request.MovementType);

        if (request.FromDate.HasValue)
            query = query.Where(im => im.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(im => im.CreatedAt <= request.ToDate.Value);

        var projected = query
            .OrderByDescending(im => im.CreatedAt)
            .Select(im => new StockMovementReportDto(
                im.Id,
                im.ProductId,
                im.Product.Name,
                im.Product.SKU,
                im.WarehouseId,
                im.Warehouse.Name,
                im.Quantity,
                im.MovementType,
                im.ReferenceId ?? string.Empty,
                im.CreatedBy,
                im.CreatedAt,
                im.Notes ?? string.Empty
            ));

        return await PaginatedList<StockMovementReportDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
