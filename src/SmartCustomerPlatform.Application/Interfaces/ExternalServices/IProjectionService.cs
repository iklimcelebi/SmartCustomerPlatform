namespace SmartCustomerPlatform.Application.Interfaces.ExternalServices;

public interface IProjectionService
{
    Task RebuildTicketProjectionAsync(
        CancellationToken cancellationToken = default);
}
