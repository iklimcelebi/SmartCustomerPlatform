using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Packages.Queries.GetPackages;

public class GetPackagesQueryHandler : IRequestHandler<GetPackagesQuery, IReadOnlyList<Package>>
{
    private readonly IPackageRepository _repository;

    public GetPackagesQueryHandler(IPackageRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<Package>> Handle(GetPackagesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}
