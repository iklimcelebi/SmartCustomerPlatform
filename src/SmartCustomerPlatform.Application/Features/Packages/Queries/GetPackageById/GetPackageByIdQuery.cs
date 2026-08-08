using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Packages.Queries.GetPackageById;

public record GetPackageByIdQuery(Guid Id) : IRequest<Package?>;
