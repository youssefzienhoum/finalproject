using MediatR;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Application.Features.Warehouses;

namespace SmartWarehouse.Application.Features.Warehouses.Commands;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, WarehouseDto>
{
    private readonly IApplicationDbContext _context;

    public CreateWarehouseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WarehouseDto> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = new Warehouse
        {
            Name = request.Name,
            Location = request.Location,
            IsActive = request.IsActive
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync(cancellationToken);

        return new WarehouseDto(warehouse.Id, warehouse.Name, warehouse.Location, warehouse.IsActive, warehouse.CreatedAt);
    }
}
