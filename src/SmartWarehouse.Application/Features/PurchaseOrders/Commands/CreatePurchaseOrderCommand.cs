using MediatR;
using SmartWarehouse.Application.Features.PurchaseOrders;

namespace SmartWarehouse.Application.Features.PurchaseOrders.Commands;

public record CreatePurchaseOrderItemCommand(int ProductId, int Quantity, decimal UnitPrice);

public record CreatePurchaseOrderCommand(
    int SupplierId,
    int WarehouseId,
    List<CreatePurchaseOrderItemCommand> Items) : IRequest<PurchaseOrderDto>;
