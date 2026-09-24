using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using SmartWarehouse.Application.Features.PurchaseOrders;

namespace SmartWarehouse.Application.Features.PurchaseOrders.Commands;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly IApplicationDbContext _context;

    public CreatePurchaseOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrderDto> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == request.WarehouseId, cancellationToken);
        if (warehouse == null)
        {
            throw new NotFoundException(nameof(Warehouse), request.WarehouseId);
        }

        var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == request.SupplierId, cancellationToken);
        if (supplier == null)
        {
            throw new NotFoundException(nameof(Supplier), request.SupplierId);
        }

        var purchaseOrder = new PurchaseOrder
        {
            SupplierId = request.SupplierId,
            WarehouseId = request.WarehouseId,
            Status = PurchaseOrderStatus.Draft,
            TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
            Items = new List<PurchaseOrderItem>()
        };

        foreach (var item in request.Items)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException(nameof(Product), item.ProductId);
            }

            purchaseOrder.Items.Add(new PurchaseOrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice
            });
        }

        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync(cancellationToken);

        // Fetch to get product details for DTO
        var createdOrder = await _context.PurchaseOrders
            .Include(po => po.Warehouse)
            .Include(po => po.Items)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstAsync(po => po.Id == purchaseOrder.Id, cancellationToken);

        return new PurchaseOrderDto(
            createdOrder.Id,
            createdOrder.SupplierId,
            createdOrder.WarehouseId,
            createdOrder.Warehouse.Name,
            createdOrder.Status,
            createdOrder.TotalAmount,
            createdOrder.CreatedAt,
            createdOrder.Items.Select(i => new PurchaseOrderItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Product.SKU,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice)).ToList());
    }
}
