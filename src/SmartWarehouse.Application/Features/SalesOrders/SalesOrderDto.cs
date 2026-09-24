using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Application.Features.SalesOrders;

public record SalesOrderItemDto(
    int Id,
    int ProductId,
    string ProductName,
    string SKU,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);

public record SalesOrderDto(
    int Id,
    int CustomerId,
    int WarehouseId,
    string WarehouseName,
    SalesOrderStatus Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    List<SalesOrderItemDto> Items);
