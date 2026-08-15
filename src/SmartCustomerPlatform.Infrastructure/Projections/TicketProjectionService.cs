using System.Text.Json;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Domain.Enums;
using SmartCustomerPlatform.Infrastructure.Elasticsearch;

namespace SmartCustomerPlatform.Infrastructure.Projections;

public class TicketProjectionService : IProjectionService
{
    private const string ElasticsearchIndex = "tickets-v1";

    private readonly IEventStoreService _eventStoreService;
    private readonly IElasticsearchService _elasticsearchService;

    public TicketProjectionService(
        IEventStoreService eventStoreService,
        IElasticsearchService elasticsearchService)
    {
        _eventStoreService = eventStoreService;
        _elasticsearchService = elasticsearchService;
    }

    public async Task RebuildTicketProjectionAsync(
        CancellationToken cancellationToken = default)
    {
        await _elasticsearchService.RecreateTicketIndexAsync(
            cancellationToken);

        var events =
            await _eventStoreService.GetAllEventsAsync(
                cancellationToken);

        var documents =
            new Dictionary<Guid, TicketDocument>();

        foreach (var storedEvent in events)
        {
            cancellationToken.ThrowIfCancellationRequested();

            JsonDocument jsonDocument;

            try
            {
                jsonDocument =
                    JsonDocument.Parse(storedEvent.Data);
            }
            catch (JsonException)
            {
                continue;
            }

            using (jsonDocument)
            {
                var root = jsonDocument.RootElement;

                if (!root.TryGetProperty(
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

                switch (storedEvent.EventType)
                {
                    case "TicketCreatedEvent":
                        ApplyCreated(
                            documents,
                            ticketId,
                            root,
                            storedEvent.CreatedAt);
                        break;

                    case "TicketAssignedEvent":
                        ApplyAssigned(
                            documents,
                            ticketId,
                            root);
                        break;

                    case "TicketStatusChangedEvent":
                        ApplyStatusChanged(
                            documents,
                            ticketId,
                            root);
                        break;

                    case "TicketPriorityChangedEvent":
                        ApplyPriorityChanged(
                            documents,
                            ticketId,
                            root);
                        break;

                    case "TicketTransferredEvent":
                        ApplyTransferred(
                            documents,
                            ticketId,
                            root);
                        break;

                    case "TicketResolvedEvent":
                        ApplyStatus(
                            documents,
                            ticketId,
                            TicketStatus.Resolved);
                        break;

                    case "TicketClosedEvent":
                        ApplyStatus(
                            documents,
                            ticketId,
                            TicketStatus.Closed);
                        break;

                    case "TicketReopenedEvent":
                        ApplyStatus(
                            documents,
                            ticketId,
                            TicketStatus.Open);
                        break;
                }
            }
        }

        foreach (var document in documents.Values)
        {
            await _elasticsearchService.IndexAsync(
                ElasticsearchIndex,
                document.TicketId.ToString(),
                document,
                cancellationToken);
        }
    }

    private static void ApplyCreated(
        Dictionary<Guid, TicketDocument> documents,
        Guid ticketId,
        JsonElement data,
        DateTime occurredOn)
    {
        var document = new TicketDocument
        {
            TicketId = ticketId,

            TicketNumber = GetString(
                data,
                "TicketNumber"),

            CustomerId = GetGuid(
                data,
                "CustomerId"),

            CustomerName = GetString(
                data,
                "CustomerName"),

            DepartmentId = GetGuid(
                data,
                "DepartmentId"),

            CategoryId = GetGuid(
                data,
                "CategoryId"),

            Subject = GetString(
                data,
                "Subject"),

            Description = GetString(
                data,
                "Description"),

            Priority = GetEnumString<TicketPriority>(
                data,
                "Priority"),

            Status = TicketStatus.Open.ToString(),

            OccurredOn = occurredOn,

            AssignedUserId = null,

            SlaStartedAt = GetDateTime(
                data,
                "SlaStartedAt",
                occurredOn),

            SlaResponseDueAt = GetDateTime(
                data,
                "SlaResponseDueAt",
                occurredOn),

            SlaResolutionDueAt = GetDateTime(
                data,
                "SlaResolutionDueAt",
                occurredOn),

            IsSlaPaused = false,

            SlaPausedAt = null,

            TotalSlaPausedDuration =
                TimeSpan.Zero
        };

        if (data.TryGetProperty(
                "SubCategoryId",
                out var subCategoryProperty)
            &&
            subCategoryProperty.ValueKind !=
            JsonValueKind.Null)
        {
            document.SubCategoryId =
                subCategoryProperty.GetGuid();
        }

        documents[ticketId] = document;
    }

    private static void ApplyAssigned(
        Dictionary<Guid, TicketDocument> documents,
        Guid ticketId,
        JsonElement data)
    {
        if (!documents.TryGetValue(
                ticketId,
                out var document))
        {
            return;
        }

        if (data.TryGetProperty(
                "AssignedUserId",
                out var assignedUserProperty)
            &&
            assignedUserProperty.ValueKind !=
            JsonValueKind.Null)
        {
            document.AssignedUserId =
                assignedUserProperty.GetGuid();
        }
    }

    private static void ApplyStatusChanged(
        Dictionary<Guid, TicketDocument> documents,
        Guid ticketId,
        JsonElement data)
    {
        if (!documents.TryGetValue(
                ticketId,
                out var document))
        {
            return;
        }

        document.Status =
            GetEnumString<TicketStatus>(
                data,
                "NewStatus");
    }

    private static void ApplyPriorityChanged(
        Dictionary<Guid, TicketDocument> documents,
        Guid ticketId,
        JsonElement data)
    {
        if (!documents.TryGetValue(
                ticketId,
                out var document))
        {
            return;
        }

        document.Priority =
            GetEnumString<TicketPriority>(
                data,
                "NewPriority");
    }

    private static void ApplyTransferred(
        Dictionary<Guid, TicketDocument> documents,
        Guid ticketId,
        JsonElement data)
    {
        if (!documents.TryGetValue(
                ticketId,
                out var document))
        {
            return;
        }

        if (data.TryGetProperty(
                "ToDepartmentId",
                out var departmentProperty))
        {
            document.DepartmentId =
                departmentProperty.GetGuid();

            document.AssignedUserId = null;
        }
    }

    private static void ApplyStatus(
        Dictionary<Guid, TicketDocument> documents,
        Guid ticketId,
        TicketStatus status)
    {
        if (!documents.TryGetValue(
                ticketId,
                out var document))
        {
            return;
        }

        document.Status = status.ToString();
    }

    private static string GetString(
        JsonElement data,
        string propertyName)
    {
        if (!data.TryGetProperty(
                propertyName,
                out var property))
        {
            return string.Empty;
        }

        return property.GetString()
            ?? string.Empty;
    }

    private static Guid GetGuid(
        JsonElement data,
        string propertyName)
    {
        if (!data.TryGetProperty(
                propertyName,
                out var property))
        {
            return Guid.Empty;
        }

        return property.GetGuid();
    }

    private static DateTime GetDateTime(
        JsonElement data,
        string propertyName,
        DateTime fallback)
    {
        if (!data.TryGetProperty(
                propertyName,
                out var property))
        {
            return fallback;
        }

        if (property.ValueKind ==
            JsonValueKind.Null)
        {
            return fallback;
        }

        return property.GetDateTime();
    }

    private static string GetEnumString<TEnum>(
        JsonElement data,
        string propertyName)
        where TEnum : struct, Enum
    {
        if (!data.TryGetProperty(
                propertyName,
                out var property))
        {
            return string.Empty;
        }

        if (property.ValueKind ==
            JsonValueKind.Number)
        {
            var value =
                property.GetInt32();

            return ((TEnum)(object)value)
                .ToString();
        }

        return property.GetString()
            ?? string.Empty;
    }
}