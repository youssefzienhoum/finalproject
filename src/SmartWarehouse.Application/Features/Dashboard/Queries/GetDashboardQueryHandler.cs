using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Application.Features.Dashboard.Queries;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var totalProductsTask = _context.Products.CountAsync(p => p.IsActive, cancellationToken);
        var totalWarehousesTask = _context.Warehouses.CountAsync(w => w.IsActive, cancellationToken);
        var totalStockQuantityTask = _context.Inventories.SumAsync(i => i.Quantity, cancellationToken);

        var inventoryItems = await _context.Inventories
            .Include(i => i.Product)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalInventoryValue = inventoryItems.Sum(i => i.Quantity * i.Product.Price);
        
        int lowStockCount = 0;
        foreach (var group in inventoryItems.GroupBy(i => i.ProductId))
        {
            var product = group.First().Product;
            var available = group.Sum(i => i.Quantity - i.ReservedQuantity);
            if (available <= product.MinimumStockLevel)
            {
                lowStockCount++;
            }
        }

        var pendingPurchasesTask = _context.PurchaseOrders
            .CountAsync(po => po.Status == PurchaseOrderStatus.Draft || po.Status == PurchaseOrderStatus.Submitted || po.Status == PurchaseOrderStatus.Approved, cancellationToken);

        var pendingSalesTask = _context.SalesOrders
            .CountAsync(so => so.Status == SalesOrderStatus.Draft || so.Status == SalesOrderStatus.Confirmed, cancellationToken);

        var completedTransfersTask = _context.StockTransfers
            .CountAsync(st => st.Status == TransferStatus.Completed, cancellationToken);

        await Task.WhenAll(
            totalProductsTask, 
            totalWarehousesTask, 
            totalStockQuantityTask, 
            pendingPurchasesTask, 
            pendingSalesTask, 
            completedTransfersTask);

        return new DashboardDto(
            totalProductsTask.Result,
            totalWarehousesTask.Result,
            totalStockQuantityTask.Result,
            totalInventoryValue,
            lowStockCount,
            pendingPurchasesTask.Result,
            pendingSalesTask.Result,
            completedTransfersTask.Result
        );
    }
}
