using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Features.SalesOrders.Commands;

public class CreateSalesOrderCommandHandler : IRequestHandler<CreateSalesOrderCommand, SalesOrderDto>
{
    private readonly IApplicationDbContext _context;

    public CreateSalesOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalesOrderDto> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == request.WarehouseId, cancellationToken);
        if (warehouse == null)
            throw new NotFoundException(nameof(Warehouse), request.WarehouseId);

        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);
        if (customer == null)
            throw new NotFoundException(nameof(Customer), request.CustomerId);

        var salesOrder = new SalesOrder
        {
            CustomerId = request.CustomerId,
            WarehouseId = request.WarehouseId,
            Status = SalesOrderStatus.Draft,
            TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
            Items = new List<SalesOrderItem>()
        };

        foreach (var item in request.Items)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), item.ProductId);

            if (!product.IsActive)
                throw new BusinessRuleException($"Product '{product.Name}' is not active.");

            salesOrder.Items.Add(new SalesOrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice
            });
        }

        _context.SalesOrders.Add(salesOrder);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.SalesOrders
            .Include(so => so.Warehouse)
            .Include(so => so.Items).ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstAsync(so => so.Id == salesOrder.Id, cancellationToken);

        return MapToDto(created);
    }

    private static SalesOrderDto MapToDto(SalesOrder so) => new(
        so.Id, so.CustomerId, so.WarehouseId, so.Warehouse.Name, so.Status, so.TotalAmount, so.CreatedAt,
        so.Items.Select(i => new SalesOrderItemDto(i.Id, i.ProductId, i.Product.Name, i.Product.SKU, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList());
}
