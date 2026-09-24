using MediatR;
using SmartWarehouse.Application.Common.Models;
using SmartWarehouse.Domain.Enums;
using SmartWarehouse.Application.Features.InventoryMovements;

namespace SmartWarehouse.Application.Features.InventoryMovements.Queries;

public record GetInventoryMovementsQuery(
    int? ProductId = null,
    int? WarehouseId = null,
    MovementType? MovementType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PaginatedList<InventoryMovementDto>>;
