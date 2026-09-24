using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Application.Features.Categories.Commands;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException(nameof(Category), request.Id);
        }

        if (category.Products.Any())
        {
            throw new BusinessRuleException("Cannot delete category because it has associated products.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
