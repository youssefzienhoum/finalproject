using MediatR;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.Inventory.Queries;

public record GetInventoryQuery(
    int? ProductId = null,
    int? WarehouseId = null,
    int? CategoryId = null,
    bool? LowStock = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PaginatedList<InventoryDto>>;
