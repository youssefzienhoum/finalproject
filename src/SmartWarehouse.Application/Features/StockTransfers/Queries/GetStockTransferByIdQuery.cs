using MediatR;

namespace SmartWarehouse.Application.Features.StockTransfers.Queries;

public record GetStockTransferByIdQuery(int Id) : IRequest<StockTransferDto>;
