using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace SmartWarehouse.Application.Features.Warehouses.Commands;

public class AssignEmployeeToWarehouseCommandHandler : IRequestHandler<AssignEmployeeToWarehouseCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public AssignEmployeeToWarehouseCommandHandler(IApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task Handle(AssignEmployeeToWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == request.WarehouseId, cancellationToken);
        if (warehouse == null)
        {
            throw new NotFoundException(nameof(Warehouse), request.WarehouseId);
        }

        var employee = await _userManager.FindByIdAsync(request.EmployeeId);
        if (employee == null)
        {
            throw new NotFoundException(nameof(User), request.EmployeeId);
        }

        var existingAssignment = await _context.WarehouseEmployees
            .FirstOrDefaultAsync(we => we.WarehouseId == request.WarehouseId && we.UserId == request.EmployeeId, cancellationToken);

        if (existingAssignment != null)
        {
            throw new BusinessRuleException("Employee is already assigned to this warehouse.");
        }

        var assignment = new WarehouseEmployee
        {
            WarehouseId = request.WarehouseId,
            UserId = request.EmployeeId
        };

        _context.WarehouseEmployees.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
