using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.Reports.Queries;

public record LowStockReportDto(
    int ProductId,
    string ProductName,
    string SKU,
    int MinimumStockLevel,
    int TotalAvailableQuantity);

public record GetLowStockReportQuery(
    int? CategoryId,
    int PageNumber = 1,
    int PageSize = 10) : MediatR.IRequest<PaginatedList<LowStockReportDto>>;
