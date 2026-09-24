using MediatR;
using SmartWarehouse.Application.Features.Warehouses;

namespace SmartWarehouse.Application.Features.Warehouses.Commands;

public record CreateWarehouseCommand(string Name, string Location, bool IsActive) : IRequest<WarehouseDto>;
