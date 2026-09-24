using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Application.Features.Warehouses;

namespace SmartWarehouse.Application.Features.Warehouses.Commands;

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, WarehouseDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateWarehouseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WarehouseDto> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (warehouse == null)
        {
            throw new NotFoundException(nameof(Warehouse), request.Id);
        }

        warehouse.Name = request.Name;
        warehouse.Location = request.Location;
        warehouse.IsActive = request.IsActive;
        warehouse.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new WarehouseDto(warehouse.Id, warehouse.Name, warehouse.Location, warehouse.IsActive, warehouse.CreatedAt);
    }
}
