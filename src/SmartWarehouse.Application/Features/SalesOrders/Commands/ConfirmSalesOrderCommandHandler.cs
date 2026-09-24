using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Features.SalesOrders.Commands;

/// <summary>
/// Confirms a sales order by reserving stock.
/// Uses optimistic concurrency (RowVersion) on Inventory rows to prevent overselling.
///
/// Race condition scenario:
///   Inventory Quantity=1, ReservedQuantity=0  →  AvailableQuantity=1
///   Employee A reads AvailableQuantity=1, tries to reserve 1
///   Employee B reads AvailableQuantity=1, tries to reserve 1
///   Without concurrency control, ReservedQuantity could become 2 → AvailableQuantity = -1 (oversold)
///
/// EF Core detects this via the RowVersion column.  When Employee B calls SaveChanges,
/// the UPDATE's WHERE clause includes the original RowVersion value.  Because Employee A
/// already changed it, zero rows are updated and EF throws DbUpdateConcurrencyException.
/// We catch this and return 409 Conflict.
/// </summary>
public class ConfirmSalesOrderCommandHandler : IRequestHandler<ConfirmSalesOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ConfirmSalesOrderCommandHandler> _logger;

    public ConfirmSalesOrderCommandHandler(IApplicationDbContext context, ILogger<ConfirmSalesOrderCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(ConfirmSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.SalesOrders
            .Include(so => so.Items)
            .FirstOrDefaultAsync(so => so.Id == request.Id, cancellationToken);

        if (order == null)
            throw new NotFoundException(nameof(SalesOrder), request.Id);

        if (order.Status != SalesOrderStatus.Draft)
            throw new BusinessRuleException("Only Draft orders can be confirmed.");

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in order.Items)
            {
                // Load inventory WITH tracking so RowVersion is checked on save
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == order.WarehouseId, cancellationToken);

                if (inventory == null)
                    throw new BusinessRuleException($"No inventory record found for Product {item.ProductId} in Warehouse {order.WarehouseId}.");

                int availableQuantity = inventory.Quantity - inventory.ReservedQuantity;

                if (availableQuantity < item.Quantity)
                    throw new BusinessRuleException($"Insufficient stock for Product {item.ProductId}. Available: {availableQuantity}, Requested: {item.Quantity}.");

                // Reserve the stock
                inventory.ReservedQuantity += item.Quantity;

                // Safety invariant: ReservedQuantity must never exceed Quantity
                if (inventory.ReservedQuantity > inventory.Quantity)
                    throw new BusinessRuleException($"ReservedQuantity ({inventory.ReservedQuantity}) would exceed Quantity ({inventory.Quantity}).");
            }

            order.Status = SalesOrderStatus.Confirmed;
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Sales Order {OrderId} confirmed by User {UserId}.", order.Id, request.UserId);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogWarning(ex, "Concurrency conflict while confirming Sales Order {OrderId}.", order.Id);
            throw new ConcurrencyException("The inventory was modified by another user. Please retry.");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
