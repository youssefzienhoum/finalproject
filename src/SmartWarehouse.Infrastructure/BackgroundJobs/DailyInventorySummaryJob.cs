using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartWarehouse.Application.Common.Interfaces;

namespace SmartWarehouse.Infrastructure.BackgroundJobs;

public class DailyInventorySummaryJob : IDailyInventorySummaryJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DailyInventorySummaryJob> _logger;

    public DailyInventorySummaryJob(IApplicationDbContext context, ILogger<DailyInventorySummaryJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating Daily Inventory Summary...");

        var totalProducts = await _context.Products.CountAsync(cancellationToken);
        
        var totalInventoryQuantity = await _context.Inventories.SumAsync(i => i.Quantity, cancellationToken);
        
        var inventoryItems = await _context.Inventories
            .Include(i => i.Product)
            .ToListAsync(cancellationToken);

        var inventoryValue = inventoryItems.Sum(i => i.Quantity * i.Product.Price);
        
        int lowStockProductsCount = 0;
        foreach (var productGroup in inventoryItems.GroupBy(i => i.ProductId))
        {
            var product = productGroup.First().Product;
            var totalAvailable = productGroup.Sum(i => i.Quantity - i.ReservedQuantity);
            
            if (totalAvailable <= product.MinimumStockLevel)
            {
                lowStockProductsCount++;
            }
        }

        _logger.LogInformation(
            "Daily Inventory Summary: Total Products: {TotalProducts}, Total Inventory Quantity: {TotalQuantity}, Inventory Value: {InventoryValue:C}, Low Stock Products: {LowStockCount}",
            totalProducts, 
            totalInventoryQuantity, 
            inventoryValue, 
            lowStockProductsCount);

        // You could also save this summary to a database table like 'InventorySummaryReport' if needed.
    }
}
