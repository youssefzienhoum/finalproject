using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Application.Features.PurchaseOrders;

public record PurchaseOrderItemDto(
    int Id,
    int ProductId,
    string ProductName,
    string SKU,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);

public record PurchaseOrderDto(
    int Id,
    int SupplierId, // Assuming a Supplier entity or just a string for now? Let's check requirements.
    int WarehouseId,
    string WarehouseName,
    PurchaseOrderStatus Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    List<PurchaseOrderItemDto> Items);
