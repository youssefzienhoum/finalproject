using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Application.Features.Warehouses.Commands;

public class RemoveEmployeeFromWarehouseCommandHandler : IRequestHandler<RemoveEmployeeFromWarehouseCommand>
{
    private readonly IApplicationDbContext _context;

    public RemoveEmployeeFromWarehouseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoveEmployeeFromWarehouseCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _context.WarehouseEmployees
            .FirstOrDefaultAsync(we => we.WarehouseId == request.WarehouseId && we.UserId == request.EmployeeId, cancellationToken);

        if (assignment == null)
        {
            throw new NotFoundException(nameof(WarehouseEmployee), $"{request.WarehouseId}-{request.EmployeeId}");
        }

        _context.WarehouseEmployees.Remove(assignment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
