using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Application.Common.Models;
using SmartWarehouse.Application.Features.InventoryMovements;

namespace SmartWarehouse.Application.Features.InventoryMovements.Queries;

public class GetInventoryMovementsQueryHandler : IRequestHandler<GetInventoryMovementsQuery, PaginatedList<InventoryMovementDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryMovementsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<InventoryMovementDto>> Handle(GetInventoryMovementsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.InventoryMovements
            .Include(m => m.Product)
            .Include(m => m.Warehouse)
            .AsNoTracking()
            .AsQueryable();

        if (request.ProductId.HasValue)
        {
            query = query.Where(m => m.ProductId == request.ProductId.Value);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(m => m.WarehouseId == request.WarehouseId.Value);
        }

        if (request.MovementType.HasValue)
        {
            query = query.Where(m => m.MovementType == request.MovementType.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(m => m.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(m => m.CreatedAt <= request.ToDate.Value);
        }

        query = query.OrderByDescending(m => m.CreatedAt);

        var dtoQuery = query.Select(m => new InventoryMovementDto(
            m.Id,
            m.ProductId,
            m.Product.Name,
            m.WarehouseId,
            m.Warehouse.Name,
            m.Quantity,
            m.MovementType,
            m.ReferenceId,
            m.CreatedBy,
            m.CreatedAt,
            m.Notes
        ));

        return await PaginatedList<InventoryMovementDto>.CreateAsync(dtoQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}
