using SmartWarehouse.Application.Common.Models;
using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Application.Features.Reports.Queries;

public record StockMovementReportDto(
    long MovementId,
    int ProductId,
    string ProductName,
    string SKU,
    int WarehouseId,
    string WarehouseName,
    int Quantity,
    MovementType MovementType,
    string ReferenceId,
    string CreatedBy,
    DateTime CreatedAt,
    string Notes);

public record GetStockMovementReportQuery(
    int? WarehouseId,
    int? ProductId,
    MovementType? MovementType,
    DateTime? FromDate,
    DateTime? ToDate,
    int PageNumber = 1,
    int PageSize = 10) : MediatR.IRequest<PaginatedList<StockMovementReportDto>>;
