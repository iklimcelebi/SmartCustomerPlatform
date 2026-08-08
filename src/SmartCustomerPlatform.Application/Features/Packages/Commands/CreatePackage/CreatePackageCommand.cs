using MediatR;

namespace SmartCustomerPlatform.Application.Features.Packages.Commands.CreatePackage;

public record CreatePackageCommand(
    string Code,
    string Name,
    string Description,
    decimal MonthlyFee,
    int TotalQuota,
    bool IsActive
) : IRequest<Guid>;
