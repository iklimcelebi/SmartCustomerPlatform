using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
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

                // 1. EventStoreDB
                await eventStoreService.AppendJsonEventAsync(
                    $"ticket-{ticketId}",
                    message.EventType,
                    message.Payload,
                    cancellationToken);

                // 2. Elasticsearch projection
                if (message.EventType == "TicketAssignedEvent")
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
                            assignedUserId
                        },
                        cancellationToken);

                    _logger.LogInformation(
                        "Elasticsearch ticket {TicketId} updated with AssignedUserId {AssignedUserId}",
                        ticketId,
                        assignedUserId);
                }

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