using MediatR;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.SalesOrders.Queries;

public record GetSalesOrdersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<SalesOrderDto>>;
