using FluentValidation;

namespace SmartCustomerPlatform.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator
    : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.TCKNo)
            .NotEmpty()
            .Length(11);

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}