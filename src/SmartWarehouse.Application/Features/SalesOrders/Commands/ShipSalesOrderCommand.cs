using MediatR;

namespace SmartWarehouse.Application.Features.SalesOrders.Commands;

/// <summary>
/// Ships a Confirmed sales order.
/// Decreases Quantity and ReservedQuantity, creates Sale movements.
/// Uses a transaction and optimistic concurrency.
/// </summary>
public record ShipSalesOrderCommand(int Id, string UserId) : IRequest;
