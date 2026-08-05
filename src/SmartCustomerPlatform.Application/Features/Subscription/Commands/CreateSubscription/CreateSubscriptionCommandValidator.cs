using FluentValidation;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.CreateSubscription;

public class CreateSubscriptionCommandValidator
    : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.PackageId)
            .NotEmpty();

        RuleFor(x => x.MonthlyPrice)
            .GreaterThan(0);

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.DiscountedPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DiscountedPrice.HasValue);
    }
}