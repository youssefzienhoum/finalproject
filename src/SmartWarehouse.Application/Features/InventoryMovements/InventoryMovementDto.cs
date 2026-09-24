using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Application.Features.InventoryMovements;

public record InventoryMovementDto(
    long Id,
    int ProductId,
    string ProductName,
    int WarehouseId,
    string WarehouseName,
    int Quantity,
    MovementType MovementType,
    string? ReferenceId,
    string CreatedBy,
    DateTime CreatedAt,
    string? Notes);
