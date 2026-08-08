using System.Text;
using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Infrastructure.EventStore;
using SmartCustomerPlatform.Persistence.Contexts;
using SmartCustomerPlatform.Persistence.Projections;
using SmartCustomerPlatform.Worker.Projections;

namespace SmartCustomerPlatform.Worker;

public class ProjectionWorker : BackgroundService
{
    private const string ProjectionName = "SubscriptionProjection";

    private readonly ILogger<ProjectionWorker> _logger;
    private readonly EventStoreReaderService _eventStoreReaderService;
    private readonly ElasticsearchClient _elasticsearchClient;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;

    public ProjectionWorker(
        ILogger<ProjectionWorker> logger,
        EventStoreReaderService eventStoreReaderService,
        ElasticsearchClient elasticsearchClient,
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _eventStoreReaderService = eventStoreReaderService;
        _elasticsearchClient = elasticsearchClient;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var indexName =
            _configuration["Elasticsearch:IndexName"]
            ?? "subscriptions-v1";

        var startPosition =
            await GetStartPositionAsync(stoppingToken);

        _logger.LogInformation(
            "Subscription projection başlatılıyor. Başlangıç pozisyonu: {Position}",
            startPosition);

        var events =
            _eventStoreReaderService.Client.ReadAllAsync(
                Direction.Forwards,
                startPosition,
                resolveLinkTos: false,
                cancellationToken: stoppingToken);

        await foreach (var resolvedEvent in events)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            try
            {
                var eventType =
                    resolvedEvent.Event.EventType;

                var json =
                    Encoding.UTF8.GetString(
                        resolvedEvent.Event.Data.Span);

                switch (eventType)
                {
                    case "SubscriptionCreatedDomainEvent":
                        await HandleSubscriptionCreatedAsync(
                            json,
                            indexName,
                            resolvedEvent,
                            stoppingToken);
                        break;

                    case "SubscriptionPackageChangedDomainEvent":
                        await HandlePackageChangedAsync(
                            json,
                            indexName,
                            resolvedEvent,
                            stoppingToken);
                        break;

                    case "SubscriptionFrozenDomainEvent":
                        await HandleFrozenAsync(
                            json,
                            indexName,
                            resolvedEvent,
                            stoppingToken);
                        break;

                    case "SubscriptionCancelledDomainEvent":
                        await HandleCancelledAsync(
                            json,
                            indexName,
                            resolvedEvent,
                            stoppingToken);
                        break;
                }

                if (resolvedEvent.Event.Position is Position eventPosition)
                {
                    await SaveCheckpointAsync(
                        eventPosition,
                        resolvedEvent.Event.EventId.ToGuid(),
                        stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Projection event işlenirken hata oluştu. EventId: {EventId}",
                    resolvedEvent.Event.EventId);
            }
        }
    }

    private async Task<Position> GetStartPositionAsync(
        CancellationToken cancellationToken)
    {
        await using var scope =
            _scopeFactory.CreateAsyncScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<SmartCustomerPlatformDbContext>();

        var checkpoint =
            await dbContext
                .Set<ProjectionCheckpoint>()
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    x => x.ProjectionName == ProjectionName,
                    cancellationToken);

        if (checkpoint is null)
        {
            _logger.LogInformation(
                "Projection checkpoint bulunamadı. Position.Start kullanılacak.");

            return Position.Start;
        }

        var position =
            new Position(
                checkpoint.CommitPosition,
                checkpoint.PreparePosition);

        _logger.LogInformation(
            "Projection checkpoint bulundu. CommitPosition: {CommitPosition}, PreparePosition: {PreparePosition}",
            checkpoint.CommitPosition,
            checkpoint.PreparePosition);

        return position;
    }

    private async Task SaveCheckpointAsync(
        Position position,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        await using var scope =
            _scopeFactory.CreateAsyncScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<SmartCustomerPlatformDbContext>();

        var checkpoint =
            await dbContext
                .Set<ProjectionCheckpoint>()
                .SingleOrDefaultAsync(
                    x => x.ProjectionName == ProjectionName,
                    cancellationToken);

        if (checkpoint is null)
        {
            checkpoint = new ProjectionCheckpoint
            {
                Id = Guid.NewGuid(),
                ProjectionName = ProjectionName,
                CommitPosition = position.CommitPosition,
                PreparePosition = position.PreparePosition,
                UpdatedAtUtc = DateTime.UtcNow,
                LastEventId = eventId,
                Status = "Running",
                ErrorMessage = null
            };

            await dbContext
                .Set<ProjectionCheckpoint>()
                .AddAsync(
                    checkpoint,
                    cancellationToken);
        }
        else
        {
            checkpoint.CommitPosition =
                position.CommitPosition;

            checkpoint.PreparePosition =
                position.PreparePosition;

            checkpoint.UpdatedAtUtc =
                DateTime.UtcNow;

            checkpoint.LastEventId =
                eventId;

            checkpoint.Status =
                "Running";

            checkpoint.ErrorMessage =
                null;
        }

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private async Task HandleSubscriptionCreatedAsync(
        string json,
        string indexName,
        ResolvedEvent resolvedEvent,
        CancellationToken stoppingToken)
    {
        var createdEvent =
            JsonSerializer.Deserialize<SubscriptionCreatedEvent>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (createdEvent is null)
            return;

        if (createdEvent.AggregateId == Guid.Empty ||
            createdEvent.CustomerId == Guid.Empty ||
            createdEvent.PackageId == Guid.Empty)
        {
            _logger.LogWarning(
                "Eksik SubscriptionCreated eventi atlandı. EventId: {EventId}",
                resolvedEvent.Event.EventId);

            return;
        }

        var document =
            new SubscriptionProjectionDocument
            {
                Id = createdEvent.AggregateId,
                CustomerId = createdEvent.CustomerId,
                PackageId = createdEvent.PackageId,
                CampaignId = createdEvent.CampaignId,
                MonthlyPrice = createdEvent.MonthlyPrice,
                DiscountedPrice = createdEvent.DiscountedPrice,
                StartDate = createdEvent.StartDate,
                EndDate = null,
                IsActive = true,
                Status = "PendingActivation",
                UpdatedAtUtc = DateTime.UtcNow
            };

        var response =
            await _elasticsearchClient.IndexAsync(
                document,
                i => i
                    .Index(indexName)
                    .Id(document.Id),
                stoppingToken);

        if (response.IsValidResponse)
        {
            _logger.LogInformation(
                "Subscription Elasticsearch'e yazıldı. SubscriptionId: {SubscriptionId}",
                document.Id);
        }
        else
        {
            throw new InvalidOperationException(
                $"Elasticsearch indexleme başarısız. SubscriptionId: {document.Id}");
        }
    }

    private async Task HandlePackageChangedAsync(
        string json,
        string indexName,
        ResolvedEvent resolvedEvent,
        CancellationToken stoppingToken)
    {
        var packageChangedEvent =
            JsonSerializer.Deserialize<SubscriptionPackageChangedProjectionEvent>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (packageChangedEvent is null)
            return;

        if (packageChangedEvent.AggregateId == Guid.Empty ||
            packageChangedEvent.PackageId == Guid.Empty)
        {
            _logger.LogWarning(
                "Eksik SubscriptionPackageChanged eventi atlandı. EventId: {EventId}",
                resolvedEvent.Event.EventId);

            return;
        }

        var getResponse =
            await _elasticsearchClient
                .GetAsync<SubscriptionProjectionDocument>(
                    packageChangedEvent.AggregateId,
                    g => g.Index(indexName),
                    stoppingToken);

        if (!getResponse.Found ||
            getResponse.Source is null)
        {
            _logger.LogWarning(
                "PackageChanged için Elasticsearch dokümanı bulunamadı. SubscriptionId: {SubscriptionId}",
                packageChangedEvent.AggregateId);

            return;
        }

        var document =
            getResponse.Source;

        document.PackageId =
            packageChangedEvent.PackageId;

        document.MonthlyPrice =
            packageChangedEvent.MonthlyPrice;

        document.UpdatedAtUtc =
            DateTime.UtcNow;

        var indexResponse =
            await _elasticsearchClient.IndexAsync(
                document,
                i => i
                    .Index(indexName)
                    .Id(document.Id),
                stoppingToken);

        if (indexResponse.IsValidResponse)
        {
            _logger.LogInformation(
                "Subscription package projection güncellendi. SubscriptionId: {SubscriptionId}",
                document.Id);
        }
        else
        {
            throw new InvalidOperationException(
                $"PackageChanged Elasticsearch güncellemesi başarısız. SubscriptionId: {document.Id}");
        }
    }

    private async Task HandleFrozenAsync(
        string json,
        string indexName,
        ResolvedEvent resolvedEvent,
        CancellationToken stoppingToken)
    {
        var frozenEvent =
            JsonSerializer.Deserialize<SubscriptionStateProjectionEvent>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (frozenEvent is null)
            return;

        if (frozenEvent.AggregateId == Guid.Empty)
        {
            _logger.LogWarning(
                "Eksik SubscriptionFrozen eventi atlandı. EventId: {EventId}",
                resolvedEvent.Event.EventId);

            return;
        }

        var getResponse =
            await _elasticsearchClient
                .GetAsync<SubscriptionProjectionDocument>(
                    frozenEvent.AggregateId,
                    g => g.Index(indexName),
                    stoppingToken);

        if (!getResponse.Found ||
            getResponse.Source is null)
        {
            _logger.LogWarning(
                "Frozen için Elasticsearch dokümanı bulunamadı. SubscriptionId: {SubscriptionId}",
                frozenEvent.AggregateId);

            return;
        }

        var document =
            getResponse.Source;

        document.IsActive =
            false;

        document.Status =
            "Frozen";

        document.UpdatedAtUtc =
            DateTime.UtcNow;

        var indexResponse =
            await _elasticsearchClient.IndexAsync(
                document,
                i => i
                    .Index(indexName)
                    .Id(document.Id),
                stoppingToken);

        if (indexResponse.IsValidResponse)
        {
            _logger.LogInformation(
                "Subscription frozen projection güncellendi. SubscriptionId: {SubscriptionId}",
                document.Id);
        }
        else
        {
            throw new InvalidOperationException(
                $"Frozen Elasticsearch güncellemesi başarısız. SubscriptionId: {document.Id}");
        }
    }

    private async Task HandleCancelledAsync(
        string json,
        string indexName,
        ResolvedEvent resolvedEvent,
        CancellationToken stoppingToken)
    {
        var cancelledEvent =
            JsonSerializer.Deserialize<SubscriptionStateProjectionEvent>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (cancelledEvent is null)
            return;

        if (cancelledEvent.AggregateId == Guid.Empty)
        {
            _logger.LogWarning(
                "Eksik SubscriptionCancelled eventi atlandı. EventId: {EventId}",
                resolvedEvent.Event.EventId);

            return;
        }

        var getResponse =
            await _elasticsearchClient
                .GetAsync<SubscriptionProjectionDocument>(
                    cancelledEvent.AggregateId,
                    g => g.Index(indexName),
                    stoppingToken);

        if (!getResponse.Found ||
            getResponse.Source is null)
        {
            _logger.LogWarning(
                "Cancelled için Elasticsearch dokümanı bulunamadı. SubscriptionId: {SubscriptionId}",
                cancelledEvent.AggregateId);

            return;
        }

        var document =
            getResponse.Source;

        document.IsActive =
            false;

        document.Status =
            "Cancelled";

        document.EndDate =
            cancelledEvent.OccurredAtUtc;

        document.UpdatedAtUtc =
            DateTime.UtcNow;

        var indexResponse =
            await _elasticsearchClient.IndexAsync(
                document,
                i => i
                    .Index(indexName)
                    .Id(document.Id),
                stoppingToken);

        if (indexResponse.IsValidResponse)
        {
            _logger.LogInformation(
                "Subscription cancelled projection güncellendi. SubscriptionId: {SubscriptionId}",
                document.Id);
        }
        else
        {
            throw new InvalidOperationException(
                $"Cancelled Elasticsearch güncellemesi başarısız. SubscriptionId: {document.Id}");
        }
    }

    private sealed class SubscriptionPackageChangedProjectionEvent
    {
        public Guid EventId { get; set; }

        public Guid AggregateId { get; set; }

        public DateTime OccurredAtUtc { get; set; }

        public string CorrelationId { get; set; } =
            string.Empty;

        public string? CausationId { get; set; }

        public string? PerformedBy { get; set; }

        public Guid PackageId { get; set; }

        public decimal MonthlyPrice { get; set; }
    }

    private sealed class SubscriptionStateProjectionEvent
    {
        public Guid EventId { get; set; }

        public Guid AggregateId { get; set; }

        public DateTime OccurredAtUtc { get; set; }

        public string CorrelationId { get; set; } =
            string.Empty;

        public string? CausationId { get; set; }

        public string? PerformedBy { get; set; }
    }
}