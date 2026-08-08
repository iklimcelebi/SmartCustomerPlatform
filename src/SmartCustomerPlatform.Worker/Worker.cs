using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Infrastructure.EventStore;
using SmartCustomerPlatform.Persistence.Contexts;
using SmartCustomerPlatform.Persistence.Outbox;

namespace SmartCustomerPlatform.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly EventStoreService _eventStoreService;

    public Worker(
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory,
        EventStoreService eventStoreService)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _eventStoreService = eventStoreService;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<SmartCustomerPlatformDbContext>();

                var now = DateTime.UtcNow;

                var pendingMessages = await dbContext.OutboxMessages
                    .Where(x =>
                        x.Status == OutboxStatus.Pending &&
                        (x.NextRetryAtUtc == null ||
                         x.NextRetryAtUtc <= now))
                    .OrderBy(x => x.OccurredAtUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);
                    

                if (pendingMessages.Count > 0)
                {
                    _logger.LogInformation(
                        "{Count} adet pending Outbox mesajı bulundu.",
                        pendingMessages.Count);
                }

                foreach (var message in pendingMessages)
                {
                    try
                    {
                        message.Status = OutboxStatus.Processing;

                        await dbContext.SaveChangesAsync(stoppingToken);

                        await _eventStoreService.AppendAsync(
                            message.AggregateId,
                            message.EventId,
                            message.EventType,
                            message.Payload,
                            stoppingToken);

                        message.Status = OutboxStatus.Processed;
                        message.ProcessedAtUtc = DateTime.UtcNow;
                        message.ErrorMessage = null;
                        message.NextRetryAtUtc = null;

                        await dbContext.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation(
                            "Outbox mesajı EventStoreDB'ye gönderildi. EventId: {EventId}",
                            message.EventId);
                    }
                    catch (Exception ex)
                    {
                        message.RetryCount++;

                        message.ErrorMessage = ex.Message;

                        if (message.RetryCount >= 5)
                        {
                            message.Status = OutboxStatus.Failed;
                            message.NextRetryAtUtc = null;
                        }
                        else
                        {
                            message.Status = OutboxStatus.Pending;
                            message.NextRetryAtUtc =
                                DateTime.UtcNow.AddSeconds(
                                    Math.Pow(2, message.RetryCount));
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);

                        _logger.LogError(
                            ex,
                            "Outbox mesajı gönderilemedi. EventId: {EventId}, RetryCount: {RetryCount}",
                            message.EventId,
                            message.RetryCount);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Outbox Worker çalışırken hata oluştu.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken);
        }
    }
}