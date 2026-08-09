using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.CreateTicket;
using SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketById;
using SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTickets;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.AddComment;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.DeleteComment;
using SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketComments;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.AssignTicket;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.ChangeStatus;
using SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketEvents;


namespace SmartCustomerPlatform.API.Controllers;
public record AddCommentRequest(
    string Content,
    string Author,
    SmartCustomerPlatform.Domain.Enums.CommentType Type
);
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

    [HttpGet("{ticketId:guid}/events")]
    public async Task<IActionResult> GetEvents(Guid ticketId)
    {
        var events = await _mediator.Send(
            new GetTicketEventsQuery(ticketId));

        return Ok(events);
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

    [HttpGet("{ticketId:guid}/comments")]
    public async Task<IActionResult> GetComments(Guid ticketId)
    {
        var comments =
            await _mediator.Send(
                new GetTicketCommentsQuery(ticketId));

        return Ok(comments);
    }

    [HttpPost("{ticketId:guid}/comments")]
    public async Task<IActionResult> AddComment(
        Guid ticketId,
        AddCommentRequest request)
    {
        var command = new AddCommentCommand(
            ticketId,
            request.Content,
            request.Author,
            request.Type);

        var commentId =
            await _mediator.Send(command);

        return Ok(commentId);
    }

    [HttpDelete("{ticketId:guid}/comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(
        Guid ticketId,
        Guid commentId)
    {
        await _mediator.Send(
            new DeleteCommentCommand(commentId));

        return NoContent();
    }
    [HttpPost("{ticketId:guid}/assign/{assignedUserId:guid}")]
    public async Task<IActionResult> Assign(
        Guid ticketId,
        Guid assignedUserId)
    {
        await _mediator.Send(
            new AssignTicketCommand(
                ticketId,
                assignedUserId));

        return NoContent();
    }



    [HttpPost("{ticketId:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid ticketId,
        SmartCustomerPlatform.Domain.Enums.TicketStatus newStatus)
    {
        await _mediator.Send(
            new ChangeStatusCommand(
                ticketId,
                newStatus));

        return NoContent();
    }



}
