using FluentValidation;

namespace SmartWarehouse.Application.Features.PurchaseOrders.Commands;

public class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.SupplierId)
            .GreaterThan(0).WithMessage("Valid SupplierId is required.");

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0).WithMessage("Valid WarehouseId is required.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ProductId).GreaterThan(0).WithMessage("Valid ProductId is required.");
            items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            items.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("UnitPrice cannot be negative.");
        });
    }
}
