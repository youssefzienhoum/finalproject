using SmartWarehouse.Application.Common.Models;
using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Application.Features.Reports.Queries;

public record SalesReportDto(
    int OrderId,
    string CustomerName,
    int TotalQuantity,
    decimal TotalAmount,
    SalesOrderStatus Status,
    DateTime CreatedAt);

public record GetSalesReportQuery(
    SalesOrderStatus? Status,
    DateTime? FromDate,
    DateTime? ToDate,
    int PageNumber = 1,
    int PageSize = 10) : MediatR.IRequest<PaginatedList<SalesReportDto>>;
