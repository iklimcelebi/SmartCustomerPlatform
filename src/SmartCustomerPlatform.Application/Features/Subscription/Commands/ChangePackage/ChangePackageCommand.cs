using MediatR;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.ChangePackage;

public record ChangePackageCommand(
    Guid Id,
    Guid PackageId,
    decimal MonthlyPrice) : IRequest<bool>;