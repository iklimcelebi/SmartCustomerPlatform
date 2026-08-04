using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Customers.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler
    : IRequestHandler<GetAllCustomersQuery, IReadOnlyList<Customer>>
{
    private readonly ICustomerRepository _customerRepository;// this means its dependent to ICustomerRepository interface. 
    //thanks to this we can use all the methods of ICustomerRepository interface in this class.

    public GetAllCustomersQueryHandler(
        ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IReadOnlyList<Customer>> Handle(
        GetAllCustomersQuery request,
        CancellationToken cancellationToken)// thanks to this if user cancels tge request the process will be stopped
    {
        return await _customerRepository.GetAllAsync();
    }
}