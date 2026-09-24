using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Application.Features.StockTransfers;

public record StockTransferItemDto(
    int Id,
    int ProductId,
    string ProductName,
    string SKU,
    int Quantity);

public record StockTransferDto(
    int Id,
    int SourceWarehouseId,
    string SourceWarehouseName,
    int DestinationWarehouseId,
    string DestinationWarehouseName,
    TransferStatus Status,
    string CreatedBy,
    DateTime CreatedAt,
    List<StockTransferItemDto> Items);
