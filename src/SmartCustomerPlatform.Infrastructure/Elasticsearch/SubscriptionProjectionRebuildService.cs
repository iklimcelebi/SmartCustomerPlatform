using System.Text;
using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using EventStore.Client;
using SmartCustomerPlatform.Application.Common.Interfaces;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.SearchSubscriptions;
using SmartCustomerPlatform.Infrastructure.EventStore;

namespace SmartCustomerPlatform.Infrastructure.Elasticsearch;

public class SubscriptionProjectionRebuildService
    : ISubscriptionProjectionRebuildService
{
    private const string IndexName = "subscriptions-v1";

    private readonly ElasticsearchClient _elasticsearchClient;
    private readonly EventStoreReaderService _eventStoreReaderService;

    public SubscriptionProjectionRebuildService(
        ElasticsearchClient elasticsearchClient,
        EventStoreReaderService eventStoreReaderService)
    {
        _elasticsearchClient = elasticsearchClient;
        _eventStoreReaderService = eventStoreReaderService;
    }

    public async Task RebuildAsync(
        CancellationToken cancellationToken = default)
    {
        var subscriptions =
            new Dictionary<Guid, SubscriptionSearchResultDto>();

        var events =
            _eventStoreReaderService.Client.ReadAllAsync(
                Direction.Forwards,
                Position.Start,
                resolveLinkTos: false,
                cancellationToken: cancellationToken);

        await foreach (var resolvedEvent in events)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var eventType =
                resolvedEvent.Event.EventType;

            var json =
                Encoding.UTF8.GetString(
                    resolvedEvent.Event.Data.Span);

            switch (eventType)
            {
                case "SubscriptionCreatedDomainEvent":
                    HandleCreated(
                        json,
                        subscriptions);
                    break;

                case "SubscriptionPackageChangedDomainEvent":
                    HandlePackageChanged(
                        json,
                        subscriptions);
                    break;

                case "SubscriptionActivatedDomainEvent":
                    HandleActivated(
                        json,
                        subscriptions);
                    break;

                case "SubscriptionFrozenDomainEvent":
                    HandleFrozen(
                        json,
                        subscriptions);
                    break;

                case "SubscriptionUnfrozenDomainEvent":
                    HandleUnfrozen(
                        json,
                        subscriptions);
                    break;

                case "SubscriptionCancelledDomainEvent":
                    HandleCancelled(
                        json,
                        subscriptions);
                    break;
            }
        }

        await DeleteIndexAsync();

        await CreateIndexAsync();

        foreach (var document in subscriptions.Values)
        {
            var response =
                await _elasticsearchClient.IndexAsync(
                    document,
                    i => i
                        .Index(IndexName)
                        .Id(document.Id),
                    cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new InvalidOperationException(
                    $"Projection rebuild sırasında Elasticsearch indexleme başarısız. SubscriptionId: {document.Id}");
            }
        }
    }

    private async Task DeleteIndexAsync()
    {
        var response =
            await _elasticsearchClient
                .Indices
                .DeleteAsync(IndexName);

        if (!response.IsValidResponse &&
            response.ElasticsearchServerError?.Status != 404)
        {
            throw new InvalidOperationException(
                "Mevcut subscription Elasticsearch indexi silinemedi.");
        }
    }

    private async Task CreateIndexAsync()
    {
        var response =
            await _elasticsearchClient
                .Indices
                .CreateAsync<SubscriptionSearchResultDto>(
                    index => index
                        .Index(IndexName)
                        .Mappings(mappings => mappings
                            .Properties(properties => properties
                                .Keyword(x => x.Id)
                                .Keyword(x => x.SubscriptionNumber)
                                .Keyword(x => x.CustomerId)
                                .Keyword(x => x.PackageId)
                                .Keyword(x => x.CampaignId)
                                .DoubleNumber(x => x.MonthlyPrice)
                                .DoubleNumber(x => x.DiscountedPrice)
                                .Date(x => x.StartDate)
                                .Date(x => x.EndDate)
                                .Boolean(x => x.IsActive)
                                .Keyword(x => x.Status)
                                .Date(x => x.UpdatedAtUtc)
                            )
                        )
                );

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                "Subscription Elasticsearch indexi yeniden oluşturulamadı.");
        }
    }

    private static void HandleCreated(
        string json,
        Dictionary<Guid, SubscriptionSearchResultDto> subscriptions)
    {
        var createdEvent =
            JsonSerializer.Deserialize<SubscriptionCreatedRebuildEvent>(
                json,
                JsonOptions);

        if (createdEvent is null ||
            createdEvent.AggregateId == Guid.Empty ||
            createdEvent.CustomerId == Guid.Empty ||
            createdEvent.PackageId == Guid.Empty)
        {
            return;
        }

        subscriptions[createdEvent.AggregateId] =
            new SubscriptionSearchResultDto
            {
                Id = createdEvent.AggregateId,

                SubscriptionNumber =
                    $"SUB-{createdEvent.AggregateId.ToString("N")[..12].ToUpperInvariant()}",

                CustomerId = createdEvent.CustomerId,
                PackageId = createdEvent.PackageId,
                CampaignId = createdEvent.CampaignId,
                MonthlyPrice = createdEvent.MonthlyPrice,
                DiscountedPrice = createdEvent.DiscountedPrice,
                StartDate = createdEvent.StartDate,
                EndDate = null,
                IsActive = true,
                Status = "PendingActivation",
                UpdatedAtUtc = createdEvent.OccurredAtUtc
            };
    }

    private static void HandlePackageChanged(
        string json,
        Dictionary<Guid, SubscriptionSearchResultDto> subscriptions)
    {
        var packageChangedEvent =
            JsonSerializer.Deserialize<SubscriptionPackageChangedRebuildEvent>(
                json,
                JsonOptions);

        if (packageChangedEvent is null ||
            !subscriptions.TryGetValue(
                packageChangedEvent.AggregateId,
                out var document))
        {
            return;
        }

        document.PackageId =
            packageChangedEvent.PackageId;

        document.MonthlyPrice =
            packageChangedEvent.MonthlyPrice;

        document.UpdatedAtUtc =
            packageChangedEvent.OccurredAtUtc;
    }

    private static void HandleActivated(
        string json,
        Dictionary<Guid, SubscriptionSearchResultDto> subscriptions)
    {
        var activatedEvent =
            JsonSerializer.Deserialize<SubscriptionStateRebuildEvent>(
                json,
                JsonOptions);

        if (activatedEvent is null ||
            !subscriptions.TryGetValue(
                activatedEvent.AggregateId,
                out var document))
        {
            return;
        }

        document.IsActive = true;
        document.Status = "Active";
        document.EndDate = null;
        document.UpdatedAtUtc =
            activatedEvent.OccurredAtUtc;
    }

    private static void HandleFrozen(
        string json,
        Dictionary<Guid, SubscriptionSearchResultDto> subscriptions)
    {
        var frozenEvent =
            JsonSerializer.Deserialize<SubscriptionStateRebuildEvent>(
                json,
                JsonOptions);

        if (frozenEvent is null ||
            !subscriptions.TryGetValue(
                frozenEvent.AggregateId,
                out var document))
        {
            return;
        }

        document.IsActive = false;
        document.Status = "Frozen";
        document.UpdatedAtUtc =
            frozenEvent.OccurredAtUtc;
    }

    private static void HandleUnfrozen(
        string json,
        Dictionary<Guid, SubscriptionSearchResultDto> subscriptions)
    {
        var unfrozenEvent =
            JsonSerializer.Deserialize<SubscriptionStateRebuildEvent>(
                json,
                JsonOptions);

        if (unfrozenEvent is null ||
            !subscriptions.TryGetValue(
                unfrozenEvent.AggregateId,
                out var document))
        {
            return;
        }

        document.IsActive = true;
        document.Status = "Active";
        document.EndDate = null;
        document.UpdatedAtUtc =
            unfrozenEvent.OccurredAtUtc;
    }

    private static void HandleCancelled(
        string json,
        Dictionary<Guid, SubscriptionSearchResultDto> subscriptions)
    {
        var cancelledEvent =
            JsonSerializer.Deserialize<SubscriptionStateRebuildEvent>(
                json,
                JsonOptions);

        if (cancelledEvent is null ||
            !subscriptions.TryGetValue(
                cancelledEvent.AggregateId,
                out var document))
        {
            return;
        }

        document.IsActive = false;
        document.Status = "Cancelled";
        document.EndDate =
            cancelledEvent.OccurredAtUtc;
        document.UpdatedAtUtc =
            cancelledEvent.OccurredAtUtc;
    }

    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true
        };

    private sealed class SubscriptionCreatedRebuildEvent
    {
        public Guid EventId { get; set; }

        public Guid AggregateId { get; set; }

        public DateTime OccurredAtUtc { get; set; }

        public string CorrelationId { get; set; } =
            string.Empty;

        public string? CausationId { get; set; }

        public string? PerformedBy { get; set; }

        public Guid CustomerId { get; set; }

        public Guid PackageId { get; set; }

        public Guid? CampaignId { get; set; }

        public decimal MonthlyPrice { get; set; }

        public decimal? DiscountedPrice { get; set; }

        public DateTime StartDate { get; set; }
    }

    private sealed class SubscriptionPackageChangedRebuildEvent
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

    private sealed class SubscriptionStateRebuildEvent
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