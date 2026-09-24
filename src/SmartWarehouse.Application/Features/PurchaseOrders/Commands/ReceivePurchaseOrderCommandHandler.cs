using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Application.Features.PurchaseOrders.Commands;

public class ReceivePurchaseOrderCommandHandler : IRequestHandler<ReceivePurchaseOrderCommand>
{
    private readonly IApplicationDbContext _context;

    public ReceivePurchaseOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ReceivePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.PurchaseOrders
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.Id == request.Id, cancellationToken);

        if (order == null)
        {
            throw new NotFoundException(nameof(PurchaseOrder), request.Id);
        }

        if (order.Status != PurchaseOrderStatus.Approved)
        {
            throw new BusinessRuleException("Only Approved orders can be received.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in order.Items)
            {
                // Update Inventory
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == order.WarehouseId, cancellationToken);

                if (inventory == null)
                {
                    inventory = new Domain.Entities.Inventory
                    {
                        ProductId = item.ProductId,
                        WarehouseId = order.WarehouseId,
                        Quantity = 0,
                        ReservedQuantity = 0
                    };
                    _context.Inventories.Add(inventory);
                }

                inventory.Quantity += item.Quantity;

                // Create Movement
                var movement = new InventoryMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = order.WarehouseId,
                    Quantity = item.Quantity,
                    MovementType = MovementType.Purchase,
                    ReferenceId = order.Id.ToString(),
                    CreatedBy = request.UserId,
                    Notes = $"Received from Purchase Order #{order.Id}"
                };
                
                _context.InventoryMovements.Add(movement);
            }

            order.Status = PurchaseOrderStatus.Received;
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
