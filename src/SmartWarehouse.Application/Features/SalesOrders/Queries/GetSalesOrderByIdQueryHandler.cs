using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Features.SalesOrders.Queries;

public class GetSalesOrderByIdQueryHandler : IRequestHandler<GetSalesOrderByIdQuery, SalesOrderDto>
{
    private readonly IApplicationDbContext _context;

    public GetSalesOrderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalesOrderDto> Handle(GetSalesOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var so = await _context.SalesOrders
            .Include(s => s.Warehouse)
            .Include(s => s.Items).ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (so == null)
            throw new NotFoundException(nameof(SalesOrder), request.Id);

        return new SalesOrderDto(
            so.Id, so.CustomerId, so.WarehouseId, so.Warehouse.Name, so.Status, so.TotalAmount, so.CreatedAt,
            so.Items.Select(i => new SalesOrderItemDto(i.Id, i.ProductId, i.Product.Name, i.Product.SKU, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList());
    }
}
