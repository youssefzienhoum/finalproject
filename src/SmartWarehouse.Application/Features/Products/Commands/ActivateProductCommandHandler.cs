using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Application.Features.Products.Commands;

public class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand>
{
    private readonly IApplicationDbContext _context;

    public ActivateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActivateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product == null)
        {
            throw new NotFoundException(nameof(Product), request.Id);
        }

        product.IsActive = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
