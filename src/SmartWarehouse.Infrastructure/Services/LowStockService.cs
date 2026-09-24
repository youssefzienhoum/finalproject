using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Services;

public class LowStockService : ILowStockService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<LowStockService> _logger;

    public LowStockService(
        IApplicationDbContext context, 
        UserManager<User> userManager,
        ILogger<LowStockService> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task CheckLowStockAndNotifyAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Low Stock Check...");

        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
        var adminId = adminUsers.FirstOrDefault()?.Id;

        if (string.IsNullOrEmpty(adminId))
        {
            _logger.LogWarning("No Admin user found to receive low stock notifications.");
            return;
        }

        var products = await _context.Products.Where(p => p.IsActive).ToListAsync(cancellationToken);

        foreach (var product in products)
        {
            var inventoryItems = await _context.Inventories
                .Where(i => i.ProductId == product.Id)
                .ToListAsync(cancellationToken);

            int totalAvailable = inventoryItems.Sum(i => i.Quantity - i.ReservedQuantity);

            if (totalAvailable <= product.MinimumStockLevel)
            {
                // Check if we already notified recently to prevent spam
                var recentNotification = await _context.Notifications
                    .Where(n => n.UserId == adminId && 
                                n.Title.Contains(product.SKU) && 
                                !n.IsRead)
                    .AnyAsync(cancellationToken);

                if (!recentNotification)
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = adminId,
                        Title = $"Low Stock Alert: {product.SKU}",
                        Message = $"Product '{product.Name}' ({product.SKU}) has fallen to {totalAvailable} available units. Minimum allowed is {product.MinimumStockLevel}.",
                        IsRead = false
                    });

                    _logger.LogInformation("Generated low stock notification for product {SKU}.", product.SKU);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Low Stock Check completed.");
    }
}
