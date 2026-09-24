using SmartWarehouse.Application.Common.Models;
using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Application.Features.Reports.Queries;

public record PurchaseReportDto(
    int OrderId,
    string SupplierName,
    int TotalQuantity,
    decimal TotalAmount,
    PurchaseOrderStatus Status,
    DateTime CreatedAt);

public record GetPurchaseReportQuery(
    PurchaseOrderStatus? Status,
    DateTime? FromDate,
    DateTime? ToDate,
    int PageNumber = 1,
    int PageSize = 10) : MediatR.IRequest<PaginatedList<PurchaseReportDto>>;
