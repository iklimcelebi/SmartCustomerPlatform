using FluentValidation;

namespace SmartCustomerPlatform.Application.Features.Packages.Commands.CreatePackage.Validators;

public class CreatePackageCommandValidator : AbstractValidator<CreatePackageCommand>
{
    public CreatePackageCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.MonthlyFee)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.TotalQuota)
            .GreaterThanOrEqualTo(0);
    }
}
