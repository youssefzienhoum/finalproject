namespace SmartWarehouse.Application.Features.Warehouses;

public record WarehouseDto(int Id, string Name, string Location, bool IsActive, DateTime CreatedAt);
