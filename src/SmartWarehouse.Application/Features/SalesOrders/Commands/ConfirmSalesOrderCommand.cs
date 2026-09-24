using MediatR;

namespace SmartWarehouse.Application.Features.SalesOrders.Commands;

/// <summary>
/// Confirms a Draft sales order by reserving inventory.
/// Increases ReservedQuantity on each Inventory row.
/// </summary>
public record ConfirmSalesOrderCommand(int Id, string UserId) : IRequest;
