using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using SmartWarehouse.Domain.Exceptions;
using System.Security.Claims;

namespace SmartWarehouse.Application.Features.StockTransfers.Commands;

public class CreateStockTransferCommandHandler : IRequestHandler<CreateStockTransferCommand, StockTransferDto>
{
    private readonly IApplicationDbContext _context;

    public CreateStockTransferCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StockTransferDto> Handle(CreateStockTransferCommand request, CancellationToken cancellationToken)
    {
        if (request.SourceWarehouseId == request.DestinationWarehouseId)
        {
            throw new BusinessRuleException("Source and destination warehouses cannot be the same.");
        }

        var sourceWarehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == request.SourceWarehouseId, cancellationToken);
        if (sourceWarehouse == null)
            throw new NotFoundException(nameof(Warehouse), request.SourceWarehouseId);
            
        var destinationWarehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == request.DestinationWarehouseId, cancellationToken);
        if (destinationWarehouse == null)
            throw new NotFoundException(nameof(Warehouse), request.DestinationWarehouseId);

        if (!destinationWarehouse.IsActive)
        {
            throw new BusinessRuleException("Destination warehouse must be active.");
        }

        var transfer = new StockTransfer
        {
            SourceWarehouseId = request.SourceWarehouseId,
            DestinationWarehouseId = request.DestinationWarehouseId,
            Status = TransferStatus.Pending,
            CreatedBy = request.UserId,
            Items = new List<StockTransferItem>()
        };

        foreach (var item in request.Items)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), item.ProductId);

            transfer.Items.Add(new StockTransferItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }

        _context.StockTransfers.Add(transfer);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.StockTransfers
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.Items).ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstAsync(t => t.Id == transfer.Id, cancellationToken);

        return new StockTransferDto(
            created.Id,
            created.SourceWarehouseId,
            created.SourceWarehouse.Name,
            created.DestinationWarehouseId,
            created.DestinationWarehouse.Name,
            created.Status,
            created.CreatedBy,
            created.CreatedAt,
            created.Items.Select(i => new StockTransferItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Product.SKU,
                i.Quantity)).ToList()
        );
    }
}
