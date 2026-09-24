using MediatR;
using SmartWarehouse.Application.Features.PurchaseOrders;

namespace SmartWarehouse.Application.Features.PurchaseOrders.Queries;

public record GetPurchaseOrderByIdQuery(int Id) : IRequest<PurchaseOrderDto>;
