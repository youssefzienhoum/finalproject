using MediatR;

namespace SmartWarehouse.Application.Features.SalesOrders.Queries;

public record GetSalesOrderByIdQuery(int Id) : IRequest<SalesOrderDto>;
