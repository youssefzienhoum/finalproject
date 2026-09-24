using MediatR;
using SmartWarehouse.Application.Features.InventoryMovements;

namespace SmartWarehouse.Application.Features.InventoryMovements.Queries;

public record GetInventoryMovementByIdQuery(long Id) : IRequest<InventoryMovementDto>;
