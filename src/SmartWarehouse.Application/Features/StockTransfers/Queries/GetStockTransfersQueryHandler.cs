using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.StockTransfers.Queries;

public class GetStockTransfersQueryHandler : IRequestHandler<GetStockTransfersQuery, PaginatedList<StockTransferDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStockTransfersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<StockTransferDto>> Handle(GetStockTransfersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockTransfers
            .Include(st => st.SourceWarehouse)
            .Include(st => st.DestinationWarehouse)
            .Include(st => st.Items).ThenInclude(i => i.Product)
            .AsNoTracking()
            .OrderByDescending(st => st.CreatedAt)
            .Select(st => new StockTransferDto(
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
            ));

        return await PaginatedList<StockTransferDto>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);
    }
}
