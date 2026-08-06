using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.Subscription.Commands.CreateSubscription;

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
            nameof(Create),
            new { id = subscriptionId },
            subscriptionId);
    }
}