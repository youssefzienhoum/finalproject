using MediatR;
using SmartWarehouse.Application.Features.StockTransfers;

namespace SmartWarehouse.Application.Features.StockTransfers.Commands;

public record CreateStockTransferItemCommand(int ProductId, int Quantity);

public record CreateStockTransferCommand(
    int SourceWarehouseId,
    int DestinationWarehouseId,
    List<CreateStockTransferItemCommand> Items,
    string UserId = "Unknown") : IRequest<StockTransferDto>;
