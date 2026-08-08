using FluentValidation;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.AddComment.Validators;

public class AddCommentCommandValidator
    : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("TicketId is required.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(2000)
            .WithMessage("Comment content is required.");

        RuleFor(x => x.Author)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid comment type.");
    }
}