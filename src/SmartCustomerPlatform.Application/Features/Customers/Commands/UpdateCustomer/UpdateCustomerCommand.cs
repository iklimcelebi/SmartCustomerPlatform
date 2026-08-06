using MediatR;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommand : IRequest<bool> // we want to se if the update was successful or not.

{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string TCKNo { get; set; } = string.Empty;

    public CustomerStatus Status { get; set; }
}