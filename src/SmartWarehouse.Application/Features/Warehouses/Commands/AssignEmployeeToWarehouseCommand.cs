using MediatR;

namespace SmartWarehouse.Application.Features.Warehouses.Commands;

public record AssignEmployeeToWarehouseCommand(int WarehouseId, string EmployeeId) : IRequest;
