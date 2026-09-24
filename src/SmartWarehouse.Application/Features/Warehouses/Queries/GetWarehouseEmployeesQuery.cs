using MediatR;
using SmartWarehouse.Application.Features.Warehouses;

namespace SmartWarehouse.Application.Features.Warehouses.Queries;

public record GetWarehouseEmployeesQuery(int WarehouseId) : IRequest<List<EmployeeDto>>;
