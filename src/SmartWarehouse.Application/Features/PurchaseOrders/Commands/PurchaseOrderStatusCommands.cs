using MediatR;

namespace SmartWarehouse.Application.Features.PurchaseOrders.Commands;

public record SubmitPurchaseOrderCommand(int Id) : IRequest;
public record ApprovePurchaseOrderCommand(int Id) : IRequest;
public record CancelPurchaseOrderCommand(int Id) : IRequest;
