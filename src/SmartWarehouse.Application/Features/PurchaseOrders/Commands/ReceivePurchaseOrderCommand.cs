using MediatR;

namespace SmartWarehouse.Application.Features.PurchaseOrders.Commands;

public record ReceivePurchaseOrderCommand(int Id, string UserId) : IRequest;
