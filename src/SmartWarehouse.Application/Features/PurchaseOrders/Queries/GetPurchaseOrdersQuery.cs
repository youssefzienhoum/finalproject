using MediatR;
using SmartWarehouse.Application.Common.Models;
using SmartWarehouse.Application.Features.PurchaseOrders;

namespace SmartWarehouse.Application.Features.PurchaseOrders.Queries;

public record GetPurchaseOrdersQuery(
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PaginatedList<PurchaseOrderDto>>;
