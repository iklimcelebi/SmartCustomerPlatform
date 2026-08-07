using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketCategories.Commands.CreateTicketCategory;

public class CreateTicketCategoryCommandHandler
    : IRequestHandler<CreateTicketCategoryCommand, Guid>
{
    private readonly ITicketCategoryRepository _repository;

    public CreateTicketCategoryCommandHandler(
        ITicketCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateTicketCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = new TicketCategory
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive
        };

        await _repository.AddAsync(category);
        await _repository.SaveChangesAsync();

        return category.Id;
    }
}
