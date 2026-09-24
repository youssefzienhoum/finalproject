namespace SmartWarehouse.Application.Features.Dashboard;

public record DashboardDto(
    int TotalProducts,
    int TotalWarehouses,
    int TotalStockQuantity,
    decimal TotalInventoryValue,
    int LowStockProducts,
    int PendingPurchaseOrders,
    int PendingSalesOrders,
    int CompletedTransfers);
