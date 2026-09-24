using FluentValidation;

namespace SmartWarehouse.Application.Features.StockTransfers.Commands;

public class CreateStockTransferCommandValidator : AbstractValidator<CreateStockTransferCommand>
{
    public CreateStockTransferCommandValidator()
    {
        RuleFor(x => x.SourceWarehouseId)
            .GreaterThan(0).WithMessage("Valid SourceWarehouseId is required.");

        RuleFor(x => x.DestinationWarehouseId)
            .GreaterThan(0).WithMessage("Valid DestinationWarehouseId is required.");

        RuleFor(x => x)
            .Must(x => x.SourceWarehouseId != x.DestinationWarehouseId)
            .WithMessage("Source and destination warehouses cannot be the same.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ProductId).GreaterThan(0).WithMessage("Valid ProductId is required.");
            items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        });
    }
}
