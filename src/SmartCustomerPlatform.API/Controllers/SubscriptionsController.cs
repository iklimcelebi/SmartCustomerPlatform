using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.Subscription.Commands.CreateSubscription;
using SmartCustomerPlatform.Application.Features.Subscription.Commands.UpdateSubscription;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.GetAllSubscriptions;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionById;

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
    public async Task<IActionResult> Create(
        CreateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var subscriptionId = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = subscriptionId },
            subscriptionId);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var subscriptions = await _mediator.Send(
            new GetAllSubscriptionsQuery(),
            cancellationToken);

        return Ok(subscriptions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var subscription = await _mediator.Send(
            new GetSubscriptionByIdQuery(id),
            cancellationToken);

        if (subscription is null)
            return NotFound();

        return Ok(subscription);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id ile request body içindeki id aynı olmalıdır.");

        var updated = await _mediator.Send(
            command,
            cancellationToken);

        if (!updated)
            return NotFound();

        return NoContent();
    }
}