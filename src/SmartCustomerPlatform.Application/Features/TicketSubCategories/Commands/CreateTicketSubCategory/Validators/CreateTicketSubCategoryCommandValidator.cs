using FluentValidation;

namespace SmartCustomerPlatform.Application.Features.TicketSubCategories.Commands.CreateTicketSubCategory.Validators;

public class CreateTicketSubCategoryCommandValidator
    : AbstractValidator<CreateTicketSubCategoryCommand>
{
    public CreateTicketSubCategoryCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.CategoryId)
            .NotEmpty();
    }
}
