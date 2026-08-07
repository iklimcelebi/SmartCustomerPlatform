using FluentValidation;

namespace SmartCustomerPlatform.Application.Features.TicketCategories.Commands.CreateTicketCategory.Validators;

public class CreateTicketCategoryCommandValidator
    : AbstractValidator<CreateTicketCategoryCommand>
{
    public CreateTicketCategoryCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
