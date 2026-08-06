using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.Customers.Commands.CreateCustomer;
using SmartCustomerPlatform.Application.Features.Customers.Queries.GetAllCustomers;
using SmartCustomerPlatform.Application.Features.Customers.Queries.GetCustomerById;

namespace SmartCustomerPlatform.API.Controllers;

[ApiController]// this means; this class is a API controller class.
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery());

        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery(id));

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCustomerCommand command)
    {
        var customerId = await _mediator.Send(command);

        return Ok(customerId);
    }
}