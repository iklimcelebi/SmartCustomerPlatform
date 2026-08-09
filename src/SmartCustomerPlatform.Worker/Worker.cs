using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Domain.Enums;
using SmartCustomerPlatform.Infrastructure.Elasticsearch;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Worker;

public class Worker : BackgroundService
{
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

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while processing outbox messages.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(2),
                stoppingToken);
        }
    }

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

        var elasticsearchService =
            scope.ServiceProvider
                .GetRequiredService<IElasticsearchService>();

        var messages = await dbContext.OutboxMessages
            .Where(x => x.ProcessedOn == null)
            .OrderBy(x => x.OccurredOn)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                _logger.LogInformation(
                    "Processing outbox message {MessageId} - {EventType}",
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

                // =====================================================
                // 1. EVENTSTOREDB
                // =====================================================

                await eventStoreService.AppendJsonEventAsync(
                    $"ticket-{ticketId}",
                    message.EventType,
                    message.Payload,
                    cancellationToken);

                // =====================================================
                // 2. ELASTICSEARCH PROJECTION
                // =====================================================

                switch (message.EventType)
                {
                    // =================================================
                    // TICKET CREATED
                    // =================================================

                    case "TicketCreatedEvent":
                    {
                        var ticketNumber =
                            eventData.GetProperty("TicketNumber")
                                .GetString() ?? string.Empty;

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
                                out var subCategoryProperty) &&
                            subCategoryProperty.ValueKind !=
                                JsonValueKind.Null)
                        {
                            subCategoryId =
                                subCategoryProperty.GetGuid();
                        }

                        var subject =
                            eventData.GetProperty("Subject")
                                .GetString() ?? string.Empty;

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
                                : DateTime.UtcNow;

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

                                // Enum -> string
                                Priority = priority.ToString(),

                                // Yeni ticket başlangıç durumu
                                Status = TicketStatus.Open.ToString(),

                                AssignedUserId = null,
                                OccurredOn = occurredOn
                            };

                        await elasticsearchService.IndexAsync(
                            "tickets",
                            ticketId.ToString(),
                            document,
                            cancellationToken);

                        _logger.LogInformation(
                            "Elasticsearch ticket {TicketId} created.",
                            ticketId);

                        break;
                    }

                    // =================================================
                    // TICKET ASSIGNED
                    // =================================================

                    case "TicketAssignedEvent":
                    {
                        if (!eventData.TryGetProperty(
                                "AssignedUserId",
                                out var assignedUserIdProperty))
                        {
                            throw new InvalidOperationException(
                                "AssignedUserId not found in TicketAssignedEvent.");
                        }

                        var assignedUserId =
                            assignedUserIdProperty.GetGuid();

                        await elasticsearchService.UpdateAsync(
                            "tickets",
                            ticketId.ToString(),
                            new
                            {
                                AssignedUserId = assignedUserId
                            },
                            cancellationToken);

                        _logger.LogInformation(
                            "Elasticsearch ticket {TicketId} updated with AssignedUserId {AssignedUserId}.",
                            ticketId,
                            assignedUserId);

                        break;
                    }

                    // =================================================
                    // TICKET TRANSFERRED
                    // =================================================

                    case "TicketTransferredEvent":
                    {
                        if (!eventData.TryGetProperty(
                                "ToDepartmentId",
                                out var departmentProperty))
                        {
                            throw new InvalidOperationException(
                                "ToDepartmentId not found in TicketTransferredEvent.");
                        }

                        var departmentId =
                            departmentProperty.GetGuid();

                        await elasticsearchService.UpdateAsync(
                            "tickets",
                            ticketId.ToString(),
                            new
                            {
                                DepartmentId = departmentId
                            },
                            cancellationToken);

                        _logger.LogInformation(
                            "Elasticsearch ticket {TicketId} department updated.",
                            ticketId);

                        break;
                    }

                    // =================================================
                    // PRIORITY CHANGED
                    // =================================================

                    case "TicketPriorityChangedEvent":
                    {
                        if (!eventData.TryGetProperty(
                                "NewPriority",
                                out var priorityProperty))
                        {
                            throw new InvalidOperationException(
                                "NewPriority not found in TicketPriorityChangedEvent.");
                        }

                        var priorityValue =
                            priorityProperty.GetInt32();

                        var priority =
                            (TicketPriority)priorityValue;

                        await elasticsearchService.UpdateAsync(
                            "tickets",
                            ticketId.ToString(),
                            new
                            {
                                Priority = priority.ToString()
                            },
                            cancellationToken);

                        _logger.LogInformation(
                            "Elasticsearch ticket {TicketId} priority updated to {Priority}.",
                            ticketId,
                            priority);

                        break;
                    }

                    // =================================================
                    // STATUS CHANGED
                    // =================================================

                    case "TicketStatusChangedEvent":
                    {
                        if (!eventData.TryGetProperty(
                                "NewStatus",
                                out var statusProperty))
                        {
                            throw new InvalidOperationException(
                                "NewStatus not found in TicketStatusChangedEvent.");
                        }

                        var statusValue =
                            statusProperty.GetInt32();

                        var status =
                            (TicketStatus)statusValue;

                        await elasticsearchService.UpdateAsync(
                            "tickets",
                            ticketId.ToString(),
                            new
                            {
                                Status = status.ToString()
                            },
                            cancellationToken);

                        _logger.LogInformation(
                            "Elasticsearch ticket {TicketId} status updated to {Status}.",
                            ticketId,
                            status);

                        break;
                    }

                    // =================================================
                    // RESOLVED
                    // =================================================

                    case "TicketResolvedEvent":
                    {
                        await elasticsearchService.UpdateAsync(
                            "tickets",
                            ticketId.ToString(),
                            new
                            {
                                Status = TicketStatus.Resolved.ToString()
                            },
                            cancellationToken);

                        _logger.LogInformation(
                            "Elasticsearch ticket {TicketId} marked as Resolved.",
                            ticketId);

                        break;
                    }

                    // =================================================
                    // CLOSED
                    // =================================================

                    case "TicketClosedEvent":
                    {
                        await elasticsearchService.UpdateAsync(
                            "tickets",
                            ticketId.ToString(),
                            new
                            {
                                Status = TicketStatus.Closed.ToString()
                            },
                            cancellationToken);

                        _logger.LogInformation(
                            "Elasticsearch ticket {TicketId} marked as Closed.",
                            ticketId);

                        break;
                    }

                    // =================================================
                    // REOPENED
                    // =================================================

                    case "TicketReopenedEvent":
                    {
                        await elasticsearchService.UpdateAsync(
                            "tickets",
                            ticketId.ToString(),
                            new
                            {
                                Status = TicketStatus.Open.ToString()
                            },
                            cancellationToken);

                        _logger.LogInformation(
                            "Elasticsearch ticket {TicketId} reopened.",
                            ticketId);

                        break;
                    }

                    // =================================================
                    // COMMENT
                    // =================================================

                    case "TicketCommentAddedEvent":
                    {
                        _logger.LogInformation(
                            "Ticket {TicketId} comment event stored in EventStoreDB. Elasticsearch projection does not require a ticket document change.",
                            ticketId);

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

                // =====================================================
                // 3. MARK OUTBOX MESSAGE AS PROCESSED
                // =====================================================

                message.ProcessedOn = DateTime.UtcNow;
                message.Error = null;

                _logger.LogInformation(
                    "Outbox message {MessageId} processed successfully.",
                    message.Id);
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message;

                _logger.LogError(
                    ex,
                    "Failed to process outbox message {MessageId}. Retry count: {RetryCount}",
                    message.Id,
                    message.RetryCount);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
