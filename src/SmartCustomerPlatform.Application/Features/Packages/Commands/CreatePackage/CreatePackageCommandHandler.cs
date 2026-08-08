using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Packages.Commands.CreatePackage;

public class CreatePackageCommandHandler : IRequestHandler<CreatePackageCommand, Guid>
{
    private readonly IPackageRepository _packageRepository;

    public CreatePackageCommandHandler(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }

    public async Task<Guid> Handle(CreatePackageCommand request, CancellationToken cancellationToken)
    {
        var package = new Package
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            MonthlyFee = request.MonthlyFee,
            TotalQuota = request.TotalQuota,
            IsActive = request.IsActive
        };

        await _packageRepository.AddAsync(package);
        await _packageRepository.SaveChangesAsync();

        return package.Id;
    }
}
