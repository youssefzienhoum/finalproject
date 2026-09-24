using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Application.Features.PurchaseOrders;

namespace SmartWarehouse.Application.Features.PurchaseOrders.Queries;

public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDto>
{
    private readonly IApplicationDbContext _context;

    public GetPurchaseOrderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrderDto> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var po = await _context.PurchaseOrders
            .Include(p => p.Warehouse)
            .Include(p => p.Items)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (po == null)
        {
            throw new NotFoundException(nameof(PurchaseOrder), request.Id);
        }

        return new PurchaseOrderDto(
            po.Id,
            po.SupplierId,
            po.WarehouseId,
            po.Warehouse.Name,
            po.Status,
            po.TotalAmount,
            po.CreatedAt,
            po.Items.Select(i => new PurchaseOrderItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Product.SKU,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice)).ToList());
    }
}
