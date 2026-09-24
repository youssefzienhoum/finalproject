using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Application.Common.Models;
using SmartWarehouse.Application.Features.PurchaseOrders;

namespace SmartWarehouse.Application.Features.PurchaseOrders.Queries;

public class GetPurchaseOrdersQueryHandler : IRequestHandler<GetPurchaseOrdersQuery, PaginatedList<PurchaseOrderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPurchaseOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<PurchaseOrderDto>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PurchaseOrders
            .Include(po => po.Warehouse)
            .Include(po => po.Items)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .OrderByDescending(po => po.CreatedAt)
            .Select(po => new PurchaseOrderDto(
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
                    i.TotalPrice)).ToList()));

        return await PaginatedList<PurchaseOrderDto>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);
    }
}
