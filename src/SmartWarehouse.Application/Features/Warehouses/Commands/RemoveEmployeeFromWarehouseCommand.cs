using MediatR;

namespace SmartWarehouse.Application.Features.Warehouses.Commands;

public record RemoveEmployeeFromWarehouseCommand(int WarehouseId, string EmployeeId) : IRequest;
