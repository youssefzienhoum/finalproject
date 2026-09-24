using MediatR;

namespace SmartWarehouse.Application.Features.SalesOrders.Commands;

public record CompleteSalesOrderCommand(int Id) : IRequest;
public record CancelSalesOrderCommand(int Id, string UserId) : IRequest;
