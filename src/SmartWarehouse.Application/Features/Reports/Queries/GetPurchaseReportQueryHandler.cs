using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.Reports.Queries;

public class GetPurchaseReportQueryHandler : IRequestHandler<GetPurchaseReportQuery, PaginatedList<PurchaseReportDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPurchaseReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<PurchaseReportDto>> Handle(GetPurchaseReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
            .AsNoTracking()
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(po => po.Status == request.Status);

        if (request.FromDate.HasValue)
            query = query.Where(po => po.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(po => po.CreatedAt <= request.ToDate.Value);

        var projected = query
            .OrderByDescending(po => po.CreatedAt)
            .Select(po => new PurchaseReportDto(
                po.Id,
                po.Supplier.Name,
                po.Items.Sum(i => i.Quantity),
                po.TotalAmount,
                po.Status,
                po.CreatedAt
            ));

        return await PaginatedList<PurchaseReportDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
