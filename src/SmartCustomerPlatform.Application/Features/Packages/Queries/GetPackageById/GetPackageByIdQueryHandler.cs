using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Packages.Queries.GetPackageById;

public class GetPackageByIdQueryHandler : IRequestHandler<GetPackageByIdQuery, Package?>
{
    private readonly IPackageRepository _repository;

    public GetPackageByIdQueryHandler(IPackageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Package?> Handle(GetPackageByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
