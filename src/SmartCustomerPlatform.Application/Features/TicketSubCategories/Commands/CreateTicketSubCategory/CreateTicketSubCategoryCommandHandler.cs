using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketSubCategories.Commands.CreateTicketSubCategory;

public class CreateTicketSubCategoryCommandHandler
    : IRequestHandler<CreateTicketSubCategoryCommand, Guid>
{
    private readonly ITicketSubCategoryRepository _repository;

    public CreateTicketSubCategoryCommandHandler(
        ITicketSubCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateTicketSubCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var subCategory = new TicketSubCategory
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive,
            CategoryId = request.CategoryId
        };

        await _repository.AddAsync(subCategory);
        await _repository.SaveChangesAsync();

        return subCategory.Id;
    }
}
