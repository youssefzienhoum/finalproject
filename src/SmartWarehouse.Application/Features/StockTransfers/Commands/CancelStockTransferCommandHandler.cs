using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Features.StockTransfers.Commands;

public class CancelStockTransferCommandHandler : IRequestHandler<CancelStockTransferCommand>
{
    private readonly IApplicationDbContext _context;

    public CancelStockTransferCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CancelStockTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = await _context.StockTransfers
            .FirstOrDefaultAsync(st => st.Id == request.Id, cancellationToken);

        if (transfer == null)
            throw new NotFoundException(nameof(StockTransfer), request.Id);

        if (transfer.Status != TransferStatus.Pending)
            throw new BusinessRuleException("Only Pending transfers can be cancelled.");

        transfer.Status = TransferStatus.Cancelled;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
