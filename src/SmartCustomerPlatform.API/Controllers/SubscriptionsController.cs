using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Common.Interfaces;
using SmartCustomerPlatform.Application.Features.Subscription.Commands.ActivateSubscription;
using SmartCustomerPlatform.Application.Features.Subscription.Commands.CancelSubscription;
using SmartCustomerPlatform.Application.Features.Subscription.Commands.CreateSubscription;
using SmartCustomerPlatform.Application.Features.Subscription.Commands.UpdateSubscription;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.GetAllSubscriptions;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionById;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionDashboard;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.SearchSubscriptions;

namespace SmartCustomerPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISubscriptionProjectionRebuildService _projectionRebuildService;

    public SubscriptionsController(
        IMediator mediator,
        ISubscriptionProjectionRebuildService projectionRebuildService)
    {
        _mediator = mediator;
        _projectionRebuildService = projectionRebuildService;
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

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken)
    {
        var dashboard = await _mediator.Send(
            new GetSubscriptionDashboardQuery(),
            cancellationToken);

        return Ok(dashboard);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] Guid? subscriptionId,
        [FromQuery] Guid? packageId,
        [FromQuery] string? status,
        [FromQuery] DateTime? startDateFrom,
        [FromQuery] DateTime? startDateTo,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SearchSubscriptionsQuery(
                subscriptionId,
                packageId,
                status,
                startDateFrom,
                startDateTo),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("projection/rebuild")]
    public async Task<IActionResult> RebuildProjection(
        CancellationToken cancellationToken)
    {
        await _projectionRebuildService.RebuildAsync(
            cancellationToken);

        return Ok(new
        {
            message = "Subscription projection başarıyla yeniden oluşturuldu."
        });
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
            return BadRequest(
                "Route id ile request body içindeki id aynı olmalıdır.");

        var updated = await _mediator.Send(
            command,
            cancellationToken);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var activated = await _mediator.Send(
            new ActivateSubscriptionCommand(id),
            cancellationToken);

        if (!activated)
            return NotFound();

        return NoContent();
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cancelled = await _mediator.Send(
            new CancelSubscriptionCommand(id),
            cancellationToken);

        if (!cancelled)
            return NotFound();

        return NoContent();
    }
}