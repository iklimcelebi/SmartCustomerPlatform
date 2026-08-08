using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.Subscriptions.Commands.CreateSubscription;
using SmartCustomerPlatform.Application.Features.Subscriptions.Queries.GetSubscriptionById;
using SmartCustomerPlatform.Application.Features.Subscriptions.Queries.GetSubscriptions;

namespace SmartCustomerPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSubscriptionCommand command)
    {
        var id = await _mediator.Send(command);

        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetSubscriptionsQuery());

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _mediator.Send(new GetSubscriptionByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
