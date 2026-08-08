using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.CreateTicket;
using SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketById;
using SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTickets;

namespace SmartCustomerPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tickets =
            await _mediator.Send(new GetTicketsQuery());

        return Ok(tickets);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ticket =
            await _mediator.Send(new GetTicketByIdQuery(id));

        if (ticket is null)
            return NotFound();

        return Ok(ticket);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTicketCommand command)
    {
        var ticketId =
            await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id = ticketId },
            ticketId);
    }
}
