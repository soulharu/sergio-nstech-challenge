using FluentValidation;

namespace Orders.Application.Commands.CreateOrder
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage("CustomerId is required.");
            RuleFor(x => x.Currency).NotEmpty().MaximumLength(3).WithMessage("Currency must be a valid 3-letter code (e.g. BRL, USD).");
            RuleFor(x => x.Items).NotEmpty().WithMessage("At least one item is required.");
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty().WithMessage("ProductId is required.");
                item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            });
        }
    }
}
