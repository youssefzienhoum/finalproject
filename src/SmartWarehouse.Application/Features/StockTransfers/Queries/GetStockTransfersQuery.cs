using MediatR;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.StockTransfers.Queries;

public record GetStockTransfersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<StockTransferDto>>;
