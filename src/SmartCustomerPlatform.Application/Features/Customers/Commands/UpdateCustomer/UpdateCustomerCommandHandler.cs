using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;

namespace SmartCustomerPlatform.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler
    : IRequestHandler<UpdateCustomerCommand, bool>
{
    private readonly ICustomerRepository _customerRepository;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<bool> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);

        if (customer is null)
            return false;

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.TCKNo = request.TCKNo;
        customer.Status = request.Status;

        _customerRepository.Update(customer);
        await _customerRepository.SaveChangesAsync();


        return true;
    }
}
// find the customer with the given id, if found then update the customer info.
//we are not creating a new customer object, we arre updating the old one.