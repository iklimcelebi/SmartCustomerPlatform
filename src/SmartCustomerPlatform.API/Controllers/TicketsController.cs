using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.AddComment;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.AssignTicket;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.ChangePriority;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.ChangeStatus;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.CreateTicket;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.DeleteComment;
using SmartCustomerPlatform.Application.Features.Tickets.Commands.TransferDepartment;
using SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketById;
using SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketComments;
using SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketEvents;
using SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTickets;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Domain.Enums;
using System.Text.Json;

namespace SmartCustomerPlatform.API.Controllers;

public record AddCommentRequest(
    string Content,
    string Author,
    CommentType Type
);

public record ChangeStatusRequest(
    string Status
);

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly IProjectionService _projectionService;
    private readonly IEventStoreService _eventStoreService;

    public TicketsController(
        IMediator mediator,
        IElasticsearchService elasticsearchService,
        IProjectionService projectionService,
        IEventStoreService eventStoreService)
    {
        _mediator = mediator;
        _elasticsearchService = elasticsearchService;
        _projectionService = projectionService;
        _eventStoreService = eventStoreService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tickets = await _mediator.Send(
            new GetTicketsQuery());

        return Ok(tickets);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? q,
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] Guid? departmentId,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? assignedUserId,
        [FromQuery] bool? slaBreached,
        CancellationToken cancellationToken)
    {
        var results =
            await _elasticsearchService.SearchTicketsAsync(
                q,
                status,
                priority,
                departmentId,
                categoryId,
                assignedUserId,
                slaBreached,
                cancellationToken);

        return Ok(results);
    }

    [HttpGet("projection")]
    public async Task<IActionResult> GetProjection(
        CancellationToken cancellationToken)
    {
        var tickets =
            await _elasticsearchService.SearchTicketsAsync(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                cancellationToken);

        var totalTickets = tickets.Count;

        var openTickets = tickets.Count(
            x => string.Equals(
                x.Status,
                TicketStatus.Open.ToString(),
                StringComparison.OrdinalIgnoreCase));

        var inProgressTickets = tickets.Count(
            x => string.Equals(
                x.Status,
                TicketStatus.InProgress.ToString(),
                StringComparison.OrdinalIgnoreCase));

        var resolvedTickets = tickets.Count(
            x => string.Equals(
                x.Status,
                TicketStatus.Resolved.ToString(),
                StringComparison.OrdinalIgnoreCase));

        var closedTickets = tickets.Count(
            x => string.Equals(
                x.Status,
                TicketStatus.Closed.ToString(),
                StringComparison.OrdinalIgnoreCase));

        var averageResolutionTime =
            await CalculateAverageResolutionTimeAsync(
                cancellationToken);

        return Ok(new
        {
            totalTickets,
            openTickets,
            closedTickets,
            resolvedTickets,
            inProgressTickets,
            averageResolutionTime
        });
    }

    [HttpPost("projection/rebuild")]
    public async Task<IActionResult> RebuildProjection(
        CancellationToken cancellationToken)
    {
        await _projectionService
            .RebuildTicketProjectionAsync(
                cancellationToken);

        return Ok(new
        {
            message =
                "Ticket projection başarıyla yeniden oluşturuldu."
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ticket = await _mediator.Send(
            new GetTicketByIdQuery(id));

        if (ticket is null)
        {
            return NotFound(new
            {
                message = "Talep bulunamadı.",
                ticketId = id
            });
        }

        var result = new
        {
            id = ticket.Id,
            ticketNumber = ticket.TicketNumber,

            subject = ticket.Subject,
            description = ticket.Description,

            status = ticket.Status.ToString(),
            priority = (int)ticket.Priority,

            customerId = ticket.CustomerId,

            customer = ticket.Customer is null
                ? null
                : new
                {
                    id = ticket.Customer.Id,
                    name =
                        $"{ticket.Customer.FirstName} {ticket.Customer.LastName}"
                            .Trim(),
                    firstName = ticket.Customer.FirstName,
                    lastName = ticket.Customer.LastName,
                    email = ticket.Customer.Email
                },

            departmentId = ticket.DepartmentId,

            department = ticket.Department is null
                ? null
                : new
                {
                    id = ticket.Department.Id,
                    name = ticket.Department.Name,
                    code = ticket.Department.Code,
                    description = ticket.Department.Description
                },

            categoryId = ticket.CategoryId,

            category = ticket.Category is null
                ? null
                : new
                {
                    id = ticket.Category.Id,
                    name = ticket.Category.Name,
                    description = ticket.Category.Description
                },

            subCategoryId = ticket.SubCategoryId,

            assignedUserId = ticket.AssignedUserId,

            createdAt = ticket.CreatedAt,
            updatedAt = ticket.UpdatedAt,

            slaStartedAt = ticket.SlaStartedAt,
            slaResponseDueAt = ticket.SlaResponseDueAt,
            slaResolutionDueAt = ticket.SlaResolutionDueAt,

            isSlaPaused = ticket.IsSlaPaused,
            slaPausedAt = ticket.SlaPausedAt,
            totalSlaPausedDuration =
                ticket.TotalSlaPausedDuration
        };

        return Ok(result);
    }

    [HttpGet("{ticketId:guid}/events")]
    public async Task<IActionResult> GetEvents(
        Guid ticketId)
    {
        var events = await _mediator.Send(
            new GetTicketEventsQuery(ticketId));

        return Ok(events);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTicketCommand command)
    {
        var ticketId = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = ticketId
            },
            ticketId);
    }

    [HttpGet("{ticketId:guid}/comments")]
    public async Task<IActionResult> GetComments(
        Guid ticketId)
    {
        var comments = await _mediator.Send(
            new GetTicketCommentsQuery(ticketId));

        return Ok(comments);
    }

    [HttpPost("{ticketId:guid}/comments")]
    public async Task<IActionResult> AddComment(
        Guid ticketId,
        [FromBody] AddCommentRequest request)
    {
        var command = new AddCommentCommand(
            ticketId,
            request.Content,
            request.Author,
            request.Type);

        var commentId = await _mediator.Send(command);

        return Ok(commentId);
    }

    [HttpDelete(
        "{ticketId:guid}/comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(
        Guid ticketId,
        Guid commentId)
    {
        await _mediator.Send(
            new DeleteCommentCommand(commentId));

        return NoContent();
    }

    [HttpPost(
        "{ticketId:guid}/assign/{assignedUserId:guid}")]
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

    [HttpPost(
        "{ticketId:guid}/transfer/{newDepartmentId:guid}")]
    public async Task<IActionResult> TransferDepartment(
        Guid ticketId,
        Guid newDepartmentId)
    {
        await _mediator.Send(
            new TransferDepartmentCommand(
                ticketId,
                newDepartmentId));

        return NoContent();
    }

    [HttpPost("{ticketId:guid}/priority")]
    public async Task<IActionResult> ChangePriority(
        Guid ticketId,
        [FromBody] TicketPriority newPriority)
    {
        try
        {
            await _mediator.Send(
                new ChangePriorityCommand(
                    ticketId,
                    newPriority));

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("{ticketId:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid ticketId,
        [FromBody] ChangeStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest(new
            {
                message = "Talep durumu boş olamaz."
            });
        }

        if (!Enum.TryParse<TicketStatus>(
                request.Status,
                true,
                out var newStatus))
        {
            return BadRequest(new
            {
                message =
                    $"Geçersiz talep durumu: {request.Status}"
            });
        }

        try
        {
            await _mediator.Send(
                new ChangeStatusCommand(
                    ticketId,
                    newStatus));

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("sla-dashboard")]
    public async Task<IActionResult> GetSlaDashboard(
        CancellationToken cancellationToken)
    {
        var tickets =
            await _elasticsearchService.SearchTicketsAsync(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                cancellationToken);

        var nowUtc = DateTime.UtcNow;

        var result = tickets
            .Select(ticket =>
            {
                var remaining =
                    ticket.SlaResolutionDueAt - nowUtc;

                var remainingMinutes =
                    Math.Max(
                        0,
                        (int)remaining.TotalMinutes);

                var isBreached =
                    ticket.SlaResponseDueAt < nowUtc
                    ||
                    ticket.SlaResolutionDueAt < nowUtc;

                return new
                {
                    id = ticket.TicketId,
                    ticketId = ticket.TicketId,
                    title = ticket.Subject,
                    status = ticket.Status,
                    remainingMinutes,
                    isBreached
                };
            })
            .OrderByDescending(
                x => x.isBreached)
            .ThenBy(
                x => x.remainingMinutes)
            .ToList();

        return Ok(result);
    }

    private async Task<double> CalculateAverageResolutionTimeAsync(
        CancellationToken cancellationToken)
    {
        var events =
            await _eventStoreService.GetAllEventsAsync(
                cancellationToken);

        var createdTimes =
            new Dictionary<Guid, DateTime>();

        var resolutionDurations =
            new List<double>();

        foreach (var storedEvent in events)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (storedEvent.EventType ==
                "TicketCreatedEvent")
            {
                try
                {
                    using var json =
                        JsonDocument.Parse(
                            storedEvent.Data);

                    if (!json.RootElement.TryGetProperty(
                            "TicketId",
                            out var ticketIdProperty))
                    {
                        continue;
                    }

                    if (!ticketIdProperty.TryGetGuid(
                            out var ticketId))
                    {
                        continue;
                    }

                    createdTimes[ticketId] =
                        storedEvent.CreatedAt;
                }
                catch (JsonException)
                {
                    continue;
                }
            }
            else if (storedEvent.EventType ==
                     "TicketResolvedEvent")
            {
                try
                {
                    using var json =
                        JsonDocument.Parse(
                            storedEvent.Data);

                    if (!json.RootElement.TryGetProperty(
                            "TicketId",
                            out var ticketIdProperty))
                    {
                        continue;
                    }

                    if (!ticketIdProperty.TryGetGuid(
                            out var ticketId))
                    {
                        continue;
                    }

                    if (!createdTimes.TryGetValue(
                            ticketId,
                            out var createdAt))
                    {
                        continue;
                    }

                    var duration =
                        storedEvent.CreatedAt - createdAt;

                    if (duration.TotalMinutes >= 0)
                    {
                        resolutionDurations.Add(
                            duration.TotalMinutes);
                    }
                }
                catch (JsonException)
                {
                    continue;
                }
            }
        }

        if (resolutionDurations.Count == 0)
        {
            return 0;
        }

        return Math.Round(
            resolutionDurations.Average(),
            2);
    }
}