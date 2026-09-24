using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Features.SalesOrders.Commands;

/// <summary>
/// Ships a confirmed sales order within a transaction:
///   1. Decrease Inventory.Quantity
///   2. Decrease Inventory.ReservedQuantity
///   3. Create InventoryMovement (Sale) for each item
///   4. Mark SalesOrder as Shipped
///   5. Commit
/// Never allows Quantity &lt; 0 or ReservedQuantity &gt; Quantity.
/// </summary>
public class ShipSalesOrderCommandHandler : IRequestHandler<ShipSalesOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ShipSalesOrderCommandHandler> _logger;

    public ShipSalesOrderCommandHandler(IApplicationDbContext context, ILogger<ShipSalesOrderCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(ShipSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.SalesOrders
            .Include(so => so.Items)
            .FirstOrDefaultAsync(so => so.Id == request.Id, cancellationToken);

        if (order == null)
            throw new NotFoundException(nameof(SalesOrder), request.Id);

        if (order.Status != SalesOrderStatus.Confirmed)
            throw new BusinessRuleException("Only Confirmed orders can be shipped.");

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in order.Items)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == order.WarehouseId, cancellationToken);

                if (inventory == null)
                    throw new BusinessRuleException($"No inventory found for Product {item.ProductId} in Warehouse {order.WarehouseId}.");

                // Decrease physical quantity and reserved quantity
                inventory.Quantity -= item.Quantity;
                inventory.ReservedQuantity -= item.Quantity;

                // Safety invariants
                if (inventory.Quantity < 0)
                    throw new BusinessRuleException($"Inventory Quantity for Product {item.ProductId} would become negative ({inventory.Quantity}).");

                if (inventory.ReservedQuantity < 0)
                    inventory.ReservedQuantity = 0; // Guard against arithmetic errors

                if (inventory.ReservedQuantity > inventory.Quantity)
                    throw new BusinessRuleException($"ReservedQuantity ({inventory.ReservedQuantity}) exceeds Quantity ({inventory.Quantity}) for Product {item.ProductId}.");

                // Create sale movement
                _context.InventoryMovements.Add(new InventoryMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = order.WarehouseId,
                    Quantity = item.Quantity,
                    MovementType = MovementType.Sale,
                    ReferenceId = order.Id.ToString(),
                    CreatedBy = request.UserId,
                    Notes = $"Shipped for Sales Order #{order.Id}"
                });
            }

            order.Status = SalesOrderStatus.Shipped;
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Sales Order {OrderId} shipped by User {UserId}.", order.Id, request.UserId);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogWarning(ex, "Concurrency conflict while shipping Sales Order {OrderId}.", order.Id);
            throw new ConcurrencyException("The inventory was modified by another user. Please retry.");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
