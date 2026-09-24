using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Application.Features.InventoryMovements;

namespace SmartWarehouse.Application.Features.InventoryMovements.Queries;

public class GetInventoryMovementByIdQueryHandler : IRequestHandler<GetInventoryMovementByIdQuery, InventoryMovementDto>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryMovementByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryMovementDto> Handle(GetInventoryMovementByIdQuery request, CancellationToken cancellationToken)
    {
        var m = await _context.InventoryMovements
            .Include(im => im.Product)
            .Include(im => im.Warehouse)
            .AsNoTracking()
            .FirstOrDefaultAsync(im => im.Id == request.Id, cancellationToken);

        if (m == null)
        {
            throw new NotFoundException(nameof(InventoryMovement), request.Id);
        }

        return new InventoryMovementDto(
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
        );
    }
}
