using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.Reports.Queries;

public record InventoryReportDto(
    int ProductId,
    string ProductName,
    string SKU,
    int WarehouseId,
    string WarehouseName,
    int Quantity,
    int ReservedQuantity,
    int AvailableQuantity);

public record GetInventoryReportQuery(
    int? WarehouseId,
    int? ProductId,
    int? CategoryId,
    int PageNumber = 1,
    int PageSize = 10) : MediatR.IRequest<PaginatedList<InventoryReportDto>>;
