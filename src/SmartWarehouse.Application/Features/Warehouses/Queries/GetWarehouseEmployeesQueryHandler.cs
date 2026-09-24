using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Application.Features.Warehouses;

namespace SmartWarehouse.Application.Features.Warehouses.Queries;

public class GetWarehouseEmployeesQueryHandler : IRequestHandler<GetWarehouseEmployeesQuery, List<EmployeeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetWarehouseEmployeesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeDto>> Handle(GetWarehouseEmployeesQuery request, CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == request.WarehouseId, cancellationToken);
        if (warehouse == null)
        {
            throw new NotFoundException(nameof(Warehouse), request.WarehouseId);
        }

        var employees = await _context.WarehouseEmployees
            .Where(we => we.WarehouseId == request.WarehouseId)
            .Include(we => we.User)
            .Select(we => new EmployeeDto(we.UserId, we.User.FirstName, we.User.LastName, we.User.Email!))
            .ToListAsync(cancellationToken);

        return employees;
    }
}
