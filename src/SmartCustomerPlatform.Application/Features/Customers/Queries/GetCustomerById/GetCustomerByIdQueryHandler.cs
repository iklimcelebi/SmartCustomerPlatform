using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler
    : IRequestHandler<GetCustomerByIdQuery, Customer?>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Customer?> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _customerRepository.GetByIdAsync(request.Id);
    }
}