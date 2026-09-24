namespace SmartWarehouse.Application.Features.Inventory;

public record InventoryDto(
    int ProductId, 
    string ProductName, 
    string SKU, 
    int WarehouseId, 
    string WarehouseName, 
    int Quantity, 
    int ReservedQuantity, 
    int AvailableQuantity);
