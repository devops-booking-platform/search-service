using MongoDB.Driver;
using SearchService.Common.Events;
using SearchService.Common.Events.Received;
using SearchService.Documents;

namespace SearchService.IntegrationEvents.Handlers;

public sealed class HostAccommodationsDeletedIntegrationEventHandler
    : IIntegrationEventHandler<HostAccommodationsDeletedIntegrationEvent>
{
    private readonly ILogger<HostAccommodationsDeletedIntegrationEventHandler> _logger;
    private readonly IMongoCollection<AccommodationDocument> _collection;

    public HostAccommodationsDeletedIntegrationEventHandler(
        ILogger<HostAccommodationsDeletedIntegrationEventHandler> logger,
        IMongoCollection<AccommodationDocument> collection)
    {
        _logger = logger;
        _collection = collection;
    }

    public async Task Handle(HostAccommodationsDeletedIntegrationEvent @event, CancellationToken ct)
    {
        var ids = @event.AccommodationIds;

        if (ids is null || ids.Count == 0)
        {
            return;
        }

        var filter = Builders<AccommodationDocument>.Filter.In(x => x.Id, ids);

        var result = await _collection.DeleteManyAsync(filter, ct);

        _logger.LogInformation(
            "Deleted {DeletedCount} accommodations from Mongo for HostAccommodationsDeletedIntegrationEvent (IdsCount={IdsCount}).",
            result.DeletedCount, ids.Count);
    }
}
