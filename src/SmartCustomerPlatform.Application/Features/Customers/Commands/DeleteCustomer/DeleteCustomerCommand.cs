using MediatR;

namespace SmartCustomerPlatform.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
