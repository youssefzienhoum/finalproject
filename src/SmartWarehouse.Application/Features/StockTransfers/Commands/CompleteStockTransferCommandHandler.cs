using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Features.StockTransfers.Commands;

public class CompleteStockTransferCommandHandler : IRequestHandler<CompleteStockTransferCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CompleteStockTransferCommandHandler> _logger;

    public CompleteStockTransferCommandHandler(IApplicationDbContext context, ILogger<CompleteStockTransferCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(CompleteStockTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = await _context.StockTransfers
            .Include(st => st.Items)
            .FirstOrDefaultAsync(st => st.Id == request.Id, cancellationToken);

        if (transfer == null)
            throw new NotFoundException(nameof(StockTransfer), request.Id);

        if (transfer.Status != TransferStatus.Pending)
            throw new BusinessRuleException("Only Pending transfers can be completed.");

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in transfer.Items)
            {
                // Source Inventory
                var sourceInventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == transfer.SourceWarehouseId, cancellationToken);

                if (sourceInventory == null)
                    throw new BusinessRuleException($"Source warehouse does not have product {item.ProductId}.");

                int availableQuantity = sourceInventory.Quantity - sourceInventory.ReservedQuantity;

                if (availableQuantity < item.Quantity)
                    throw new BusinessRuleException($"Insufficient stock for product {item.ProductId} in source warehouse.");

                sourceInventory.Quantity -= item.Quantity;

                _context.InventoryMovements.Add(new InventoryMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = transfer.SourceWarehouseId,
                    Quantity = item.Quantity,
                    MovementType = MovementType.TransferOut,
                    ReferenceId = transfer.Id.ToString(),
                    CreatedBy = request.UserId,
                    Notes = $"Stock transfer out #{transfer.Id}"
                });

                // Destination Inventory
                var destInventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == transfer.DestinationWarehouseId, cancellationToken);

                if (destInventory == null)
                {
                    destInventory = new Domain.Entities.Inventory
                    {
                        ProductId = item.ProductId,
                        WarehouseId = transfer.DestinationWarehouseId,
                        Quantity = 0,
                        ReservedQuantity = 0
                    };
                    _context.Inventories.Add(destInventory);
                }

                destInventory.Quantity += item.Quantity;

                _context.InventoryMovements.Add(new InventoryMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = transfer.DestinationWarehouseId,
                    Quantity = item.Quantity,
                    MovementType = MovementType.TransferIn,
                    ReferenceId = transfer.Id.ToString(),
                    CreatedBy = request.UserId,
                    Notes = $"Stock transfer in #{transfer.Id}"
                });
            }

            transfer.Status = TransferStatus.Completed;
            
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            _logger.LogInformation("Stock Transfer {TransferId} completed by User {UserId}.", transfer.Id, request.UserId);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogWarning(ex, "Concurrency conflict while completing Stock Transfer {TransferId}.", transfer.Id);
            throw new ConcurrencyException("The inventory was modified by another user. Please retry.");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
