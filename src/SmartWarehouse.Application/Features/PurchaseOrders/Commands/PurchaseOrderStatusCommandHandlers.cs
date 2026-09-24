using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Application.Features.PurchaseOrders.Commands;

public class PurchaseOrderStatusCommandHandlers : 
    IRequestHandler<SubmitPurchaseOrderCommand>,
    IRequestHandler<ApprovePurchaseOrderCommand>,
    IRequestHandler<CancelPurchaseOrderCommand>
{
    private readonly IApplicationDbContext _context;

    public PurchaseOrderStatusCommandHandlers(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(SubmitPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await GetOrderAsync(request.Id, cancellationToken);
        if (order.Status != PurchaseOrderStatus.Draft)
        {
            throw new BusinessRuleException("Only Draft orders can be submitted.");
        }
        order.Status = PurchaseOrderStatus.Submitted;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(ApprovePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await GetOrderAsync(request.Id, cancellationToken);
        if (order.Status != PurchaseOrderStatus.Submitted)
        {
            throw new BusinessRuleException("Only Submitted orders can be approved.");
        }
        order.Status = PurchaseOrderStatus.Approved;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(CancelPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await GetOrderAsync(request.Id, cancellationToken);
        if (order.Status == PurchaseOrderStatus.Received || order.Status == PurchaseOrderStatus.Cancelled)
        {
            throw new BusinessRuleException("Cannot cancel a received or already cancelled order.");
        }
        order.Status = PurchaseOrderStatus.Cancelled;
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<PurchaseOrder> GetOrderAsync(int id, CancellationToken cancellationToken)
    {
        var order = await _context.PurchaseOrders.FirstOrDefaultAsync(po => po.Id == id, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(PurchaseOrder), id);
        }
        return order;
    }
}
