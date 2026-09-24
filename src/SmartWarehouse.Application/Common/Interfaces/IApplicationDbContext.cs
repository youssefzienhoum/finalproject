using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Application.Common.Interfaces;

/// <summary>
/// Interface for the Entity Framework Core DbContext.
/// This allows the Application layer to query and modify the database without depending on the Infrastructure layer.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<Warehouse> Warehouses { get; }
    DbSet<WarehouseEmployee> WarehouseEmployees { get; }
    DbSet<Inventory> Inventories { get; }
    DbSet<InventoryMovement> InventoryMovements { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseOrderItem> PurchaseOrderItems { get; }
    DbSet<Customer> Customers { get; }
    DbSet<SalesOrder> SalesOrders { get; }
    DbSet<SalesOrderItem> SalesOrderItems { get; }
    DbSet<StockTransfer> StockTransfers { get; }
    DbSet<StockTransferItem> StockTransferItems { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
