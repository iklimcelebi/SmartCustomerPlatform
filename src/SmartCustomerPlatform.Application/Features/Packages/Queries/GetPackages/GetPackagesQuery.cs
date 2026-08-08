using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Packages.Queries.GetPackages;

public record GetPackagesQuery : IRequest<IReadOnlyList<Package>>;
