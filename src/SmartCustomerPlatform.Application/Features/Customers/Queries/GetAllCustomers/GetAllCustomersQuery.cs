using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery() : IRequest<IReadOnlyList<Customer>>;
// record is a reference type that provides built-in functionality for encapsulating data. It is used to define immutable data structures with value-based equality semantics. 
