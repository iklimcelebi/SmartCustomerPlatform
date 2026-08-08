using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.Campaigns.Commands.CreateCampaign;
using SmartCustomerPlatform.Application.Features.Campaigns.Queries.GetCampaignById;
using SmartCustomerPlatform.Application.Features.Campaigns.Queries.GetCampaigns;

namespace SmartCustomerPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampaignsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCampaignCommand command)
    {
        var id = await _mediator.Send(command);

        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetCampaignsQuery());

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _mediator.Send(new GetCampaignByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
