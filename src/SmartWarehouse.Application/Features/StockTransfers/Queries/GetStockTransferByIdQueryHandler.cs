using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Features.StockTransfers.Queries;

public class GetStockTransferByIdQueryHandler : IRequestHandler<GetStockTransferByIdQuery, StockTransferDto>
{
    private readonly IApplicationDbContext _context;

    public GetStockTransferByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StockTransferDto> Handle(GetStockTransferByIdQuery request, CancellationToken cancellationToken)
    {
        var st = await _context.StockTransfers
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.Items).ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (st == null)
            throw new NotFoundException(nameof(StockTransfer), request.Id);

        return new StockTransferDto(
            st.Id,
            st.SourceWarehouseId,
            st.SourceWarehouse.Name,
            st.DestinationWarehouseId,
            st.DestinationWarehouse.Name,
            st.Status,
            st.CreatedBy,
            st.CreatedAt,
            st.Items.Select(i => new StockTransferItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Product.SKU,
                i.Quantity)).ToList()
        );
    }
}
