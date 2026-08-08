using FluentValidation;

namespace SmartCustomerPlatform.Application.Features.Subscriptions.Commands.CreateSubscription.Validators;

public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionNumber)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.PackageName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.MonthlyFee)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.UsedQuota)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.RemainingQuota)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.TotalQuota)
            .GreaterThanOrEqualTo(0);
    }
}
