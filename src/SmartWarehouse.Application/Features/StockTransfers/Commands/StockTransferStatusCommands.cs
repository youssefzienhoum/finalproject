using MediatR;

namespace SmartWarehouse.Application.Features.StockTransfers.Commands;

public record CompleteStockTransferCommand(int Id, string UserId) : IRequest;
public record CancelStockTransferCommand(int Id, string UserId) : IRequest;
