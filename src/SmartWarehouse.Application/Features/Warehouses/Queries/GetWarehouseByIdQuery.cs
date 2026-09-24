using MediatR;
using SmartWarehouse.Application.Features.Warehouses;

namespace SmartWarehouse.Application.Features.Warehouses.Queries;

public record GetWarehouseByIdQuery(int Id) : IRequest<WarehouseDto>;
