using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Features.SalesOrders.Commands;

public class SalesOrderStatusCommandHandlers :
    IRequestHandler<CompleteSalesOrderCommand>,
    IRequestHandler<CancelSalesOrderCommand>
{
    private readonly IApplicationDbContext _context;

    public SalesOrderStatusCommandHandlers(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Complete: Only Shipped orders can be completed. No inventory changes needed.
    /// </summary>
    public async Task Handle(CompleteSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await GetOrderAsync(request.Id, cancellationToken);
        if (order.Status != SalesOrderStatus.Shipped)
            throw new BusinessRuleException("Only Shipped orders can be completed.");

        order.Status = SalesOrderStatus.Completed;
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Cancel: Draft or Confirmed orders can be cancelled.
    /// If Confirmed, we must release reserved stock (decrease ReservedQuantity).
    /// Shipped/Completed orders cannot be cancelled.
    /// </summary>
    public async Task Handle(CancelSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.SalesOrders
            .Include(so => so.Items)
            .FirstOrDefaultAsync(so => so.Id == request.Id, cancellationToken);

        if (order == null)
            throw new NotFoundException(nameof(SalesOrder), request.Id);

        if (order.Status == SalesOrderStatus.Shipped || order.Status == SalesOrderStatus.Completed || order.Status == SalesOrderStatus.Cancelled)
            throw new BusinessRuleException("Cannot cancel a shipped, completed, or already cancelled order.");

        // If confirmed, release reserved inventory
        if (order.Status == SalesOrderStatus.Confirmed)
        {
            foreach (var item in order.Items)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == order.WarehouseId, cancellationToken);

                if (inventory != null)
                {
                    inventory.ReservedQuantity -= item.Quantity;
                    if (inventory.ReservedQuantity < 0)
                        inventory.ReservedQuantity = 0;
                }
            }
        }

        order.Status = SalesOrderStatus.Cancelled;
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<SalesOrder> GetOrderAsync(int id, CancellationToken cancellationToken)
    {
        var order = await _context.SalesOrders.FirstOrDefaultAsync(so => so.Id == id, cancellationToken);
        if (order == null)
            throw new NotFoundException(nameof(SalesOrder), id);
        return order;
    }
}
