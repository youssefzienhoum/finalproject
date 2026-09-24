using MediatR;

namespace SmartWarehouse.Application.Features.SalesOrders.Commands;

public record CreateSalesOrderItemCommand(int ProductId, int Quantity, decimal UnitPrice);

public record CreateSalesOrderCommand(
    int CustomerId,
    int WarehouseId,
    List<CreateSalesOrderItemCommand> Items) : IRequest<SalesOrderDto>;
