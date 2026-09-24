using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.Reports.Queries;

public class GetSalesReportQueryHandler : IRequestHandler<GetSalesReportQuery, PaginatedList<SalesReportDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSalesReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<SalesReportDto>> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.Items)
            .AsNoTracking()
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(so => so.Status == request.Status);

        if (request.FromDate.HasValue)
            query = query.Where(so => so.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(so => so.CreatedAt <= request.ToDate.Value);

        var projected = query
            .OrderByDescending(so => so.CreatedAt)
            .Select(so => new SalesReportDto(
                so.Id,
                so.Customer.Name,
                so.Items.Sum(i => i.Quantity),
                so.TotalAmount,
                so.Status,
                so.CreatedAt
            ));

        return await PaginatedList<SalesReportDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
