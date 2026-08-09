using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Domain.Enums;
using SmartCustomerPlatform.Infrastructure.Elasticsearch;
using SmartCustomerPlatform.Persistence.Contexts;
using SmartCustomerPlatform.Persistence.Outbox;

namespace SmartCustomerPlatform.Worker;

public class Worker : BackgroundService
{
    private const string ProjectionName = "TicketSearchProjection";
    private const string ElasticsearchIndex = "tickets-v1";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IServiceScopeFactory scopeFactory,
        ILogger<Worker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Worker started.");

        // Elasticsearch indexini oluştur
        using (var scope = _scopeFactory.CreateScope())
        {
            var elasticsearchService =
                scope.ServiceProvider
                    .GetRequiredService<IElasticsearchService>();

            await elasticsearchService.CreateTicketIndexAsync(
                stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 1. Outbox -> EventStoreDB
                await ProcessOutboxMessages(stoppingToken);

                // 2. Outbox -> Elasticsearch Projection
                await ProcessElasticsearchProjection(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while processing worker operations.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(2),
                stoppingToken);
        }
    }

    // ============================================================
    // 1. OUTBOX -> EVENTSTOREDB
    // ============================================================

    private async Task ProcessOutboxMessages(
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<SmartCustomerPlatformDbContext>();

        var eventStoreService =
            scope.ServiceProvider
                .GetRequiredService<IEventStoreService>();

        var messages = await dbContext.OutboxMessages
            .Where(x => x.ProcessedOn == null)
            .OrderBy(x => x.OccurredOn)
            .ThenBy(x => x.Id)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                _logger.LogInformation(
                    "Processing EventStoreDB outbox message {MessageId} - {EventType}",
                    message.Id,
                    message.EventType);

                var eventData =
                    JsonSerializer.Deserialize<JsonElement>(
                        message.Payload);

                if (!eventData.TryGetProperty(
                        "TicketId",
                        out var ticketIdProperty))
                {
                    throw new InvalidOperationException(
                        $"TicketId not found in event {message.EventType}");
                }

                var ticketId = ticketIdProperty.GetGuid();

                await eventStoreService.AppendJsonEventAsync(
                    $"ticket-{ticketId}",
                    message.EventType,
                    message.Payload,
                    cancellationToken);

                message.ProcessedOn = DateTime.UtcNow;
                message.Error = null;

                _logger.LogInformation(
                    "Outbox message {MessageId} successfully stored in EventStoreDB.",
                    message.Id);
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message;

                _logger.LogError(
                    ex,
                    "Failed to process EventStoreDB outbox message {MessageId}. Retry count: {RetryCount}",
                    message.Id,
                    message.RetryCount);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    // ============================================================
    // 2. OUTBOX -> ELASTICSEARCH PROJECTION
    // ============================================================

    private async Task ProcessElasticsearchProjection(
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<SmartCustomerPlatformDbContext>();

        var elasticsearchService =
            scope.ServiceProvider
                .GetRequiredService<IElasticsearchService>();

        // --------------------------------------------------------
        // CHECKPOINT GET / CREATE
        // --------------------------------------------------------

        var checkpoint =
            await dbContext.ProjectionCheckpoints
                .FirstOrDefaultAsync(
                    x => x.ProjectionName == ProjectionName,
                    cancellationToken);

        if (checkpoint == null)
        {
            checkpoint = new ProjectionCheckpoint
            {
                Id = Guid.NewGuid(),
                ProjectionName = ProjectionName,
                LastProcessedOccurredOn = DateTime.MinValue,
                LastProcessedMessageId = Guid.Empty
            };

            dbContext.ProjectionCheckpoints.Add(checkpoint);

            await dbContext.SaveChangesAsync(
                cancellationToken);

            _logger.LogInformation(
                "Projection checkpoint created for {ProjectionName}.",
                ProjectionName);
        }

        // --------------------------------------------------------
        // CHECKPOINT'TEN SONRAKİ EVENTLERİ AL
        // --------------------------------------------------------

        var messages = await dbContext.OutboxMessages
            .Where(x =>
                x.OccurredOn > checkpoint.LastProcessedOccurredOn
                ||
                (
                    x.OccurredOn == checkpoint.LastProcessedOccurredOn
                    &&
                    x.Id.CompareTo(
                        checkpoint.LastProcessedMessageId) > 0
                ))
            .OrderBy(x => x.OccurredOn)
            .ThenBy(x => x.Id)
            .Take(20)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            return;
        }

        foreach (var message in messages)
        {
            try
            {
                _logger.LogInformation(
                    "Projecting message {MessageId} - {EventType} to Elasticsearch.",
                    message.Id,
                    message.EventType);

                var eventData =
                    JsonSerializer.Deserialize<JsonElement>(
                        message.Payload);

                if (!eventData.TryGetProperty(
                        "TicketId",
                        out var ticketIdProperty))
                {
                    throw new InvalidOperationException(
                        $"TicketId not found in event {message.EventType}");
                }

                var ticketId =
                    ticketIdProperty.GetGuid();

                switch (message.EventType)
                {
                    // =================================================
                    // TICKET CREATED
                    // =================================================

                    case "TicketCreatedEvent":
                    {
                        var ticketNumber =
                            eventData.GetProperty("TicketNumber")
                                .GetString()
                            ?? string.Empty;

                        var customerId =
                            eventData.GetProperty("CustomerId")
                                .GetGuid();

                        var departmentId =
                            eventData.GetProperty("DepartmentId")
                                .GetGuid();

                        var categoryId =
                            eventData.GetProperty("CategoryId")
                                .GetGuid();

                        Guid? subCategoryId = null;

                        if (eventData.TryGetProperty(
                                "SubCategoryId",
                                out var subCategoryProperty)
                            &&
                            subCategoryProperty.ValueKind !=
                            JsonValueKind.Null)
                        {
                            subCategoryId =
                                subCategoryProperty.GetGuid();
                        }

                        var subject =
                            eventData.GetProperty("Subject")
                                .GetString()
                            ?? string.Empty;

                        var priorityValue =
                            eventData.GetProperty("Priority")
                                .GetInt32();

                        var priority =
                            (TicketPriority)priorityValue;

                        var occurredOn =
                            eventData.TryGetProperty(
                                "OccurredOn",
                                out var occurredOnProperty)
                                ? occurredOnProperty.GetDateTime()
                                : message.OccurredOn;

                        var document =
                            new TicketDocument
                            {
                                TicketId = ticketId,
                                TicketNumber = ticketNumber,
                                CustomerId = customerId,
                                DepartmentId = departmentId,
                                CategoryId = categoryId,
                                SubCategoryId = subCategoryId,
                                Subject = subject,
                                Priority = priority.ToString(),
                                Status = TicketStatus.Open.ToString(),
                                AssignedUserId = null,
                                OccurredOn = occurredOn
                            };

                        await elasticsearchService.IndexAsync(
                            ElasticsearchIndex,
                            ticketId.ToString(),
                            document,
                            cancellationToken);

                        _logger.LogInformation(
                            "Ticket {TicketId} projected to Elasticsearch.",
                            ticketId);

                        break;
                    }

                    // =================================================
                    // ASSIGNED
                    // =================================================

                    case "TicketAssignedEvent":
                    {
                        var assignedUserId =
                            eventData
                                .GetProperty("AssignedUserId")
                                .GetGuid();

                        await elasticsearchService.UpdateAsync(
                            ElasticsearchIndex,
                            ticketId.ToString(),
                            new
                            {
                                AssignedUserId = assignedUserId
                            },
                            cancellationToken);

                        break;
                    }

                    // =================================================
                    // TRANSFERRED
                    // =================================================

                    case "TicketTransferredEvent":
                    {
                        var departmentId =
                            eventData
                                .GetProperty("ToDepartmentId")
                                .GetGuid();

                        await elasticsearchService.UpdateAsync(
                            ElasticsearchIndex,
                            ticketId.ToString(),
                            new
                            {
                                DepartmentId = departmentId
                            },
                            cancellationToken);

                        break;
                    }

                    // =================================================
                    // PRIORITY CHANGED
                    // =================================================

                    case "TicketPriorityChangedEvent":
                    {
                        var priorityValue =
                            eventData
                                .GetProperty("NewPriority")
                                .GetInt32();

                        var priority =
                            (TicketPriority)priorityValue;

                        await elasticsearchService.UpdateAsync(
                            ElasticsearchIndex,
                            ticketId.ToString(),
                            new
                            {
                                Priority = priority.ToString()
                            },
                            cancellationToken);

                        break;
                    }

                    // =================================================
                    // STATUS CHANGED
                    // =================================================

                    case "TicketStatusChangedEvent":
                    {
                        var statusValue =
                            eventData
                                .GetProperty("NewStatus")
                                .GetInt32();

                        var status =
                            (TicketStatus)statusValue;

                        await elasticsearchService.UpdateAsync(
                            ElasticsearchIndex,
                            ticketId.ToString(),
                            new
                            {
                                Status = status.ToString()
                            },
                            cancellationToken);

                        break;
                    }

                    // =================================================
                    // RESOLVED
                    // =================================================

                    case "TicketResolvedEvent":
                    {
                        await elasticsearchService.UpdateAsync(
                            ElasticsearchIndex,
                            ticketId.ToString(),
                            new
                            {
                                Status =
                                    TicketStatus.Resolved.ToString()
                            },
                            cancellationToken);

                        break;
                    }

                    // =================================================
                    // CLOSED
                    // =================================================

                    case "TicketClosedEvent":
                    {
                        await elasticsearchService.UpdateAsync(
                            ElasticsearchIndex,
                            ticketId.ToString(),
                            new
                            {
                                Status =
                                    TicketStatus.Closed.ToString()
                            },
                            cancellationToken);

                        break;
                    }

                    // =================================================
                    // REOPENED
                    // =================================================

                    case "TicketReopenedEvent":
                    {
                        await elasticsearchService.UpdateAsync(
                            ElasticsearchIndex,
                            ticketId.ToString(),
                            new
                            {
                                Status =
                                    TicketStatus.Open.ToString()
                            },
                            cancellationToken);

                        break;
                    }

                    // =================================================
                    // COMMENT
                    // =================================================

                    case "TicketCommentAddedEvent":
                    {
                        _logger.LogInformation(
                            "Comment event {MessageId} stored in EventStoreDB. No Elasticsearch document update required.",
                            message.Id);

                        break;
                    }

                    // =================================================
                    // UNKNOWN EVENT
                    // =================================================

                    default:
                    {
                        _logger.LogWarning(
                            "Event type {EventType} has no Elasticsearch projection handler.",
                            message.EventType);

                        break;
                    }
                }

                // ----------------------------------------------------
                // CHECKPOINT UPDATE
                // ----------------------------------------------------

                checkpoint.LastProcessedOccurredOn =
                    message.OccurredOn;

                checkpoint.LastProcessedMessageId =
                    message.Id;

                await dbContext.SaveChangesAsync(
                    cancellationToken);

                _logger.LogInformation(
                    "Projection checkpoint updated. MessageId: {MessageId}",
                    message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to project message {MessageId} to Elasticsearch.",
                    message.Id);

                // Bu event başarısızsa checkpoint ilerletilmiyor.
                // Böylece bir sonraki turda tekrar deneniyor.
                break;
            }
        }
    }
}