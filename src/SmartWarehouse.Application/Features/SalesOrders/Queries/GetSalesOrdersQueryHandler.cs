using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.SalesOrders.Queries;

public class GetSalesOrdersQueryHandler : IRequestHandler<GetSalesOrdersQuery, PaginatedList<SalesOrderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSalesOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<SalesOrderDto>> Handle(GetSalesOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SalesOrders
            .Include(so => so.Warehouse)
            .Include(so => so.Items).ThenInclude(i => i.Product)
            .AsNoTracking()
            .OrderByDescending(so => so.CreatedAt)
            .Select(so => new SalesOrderDto(
                so.Id, so.CustomerId, so.WarehouseId, so.Warehouse.Name, so.Status, so.TotalAmount, so.CreatedAt,
                so.Items.Select(i => new SalesOrderItemDto(i.Id, i.ProductId, i.Product.Name, i.Product.SKU, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList()));

        return await PaginatedList<SalesOrderDto>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);
    }
}
