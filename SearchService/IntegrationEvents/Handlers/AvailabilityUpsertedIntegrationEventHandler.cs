using MongoDB.Driver;
using SearchService.Common.Events;
using SearchService.Common.Events.Received;
using SearchService.Documents;

namespace SearchService.IntegrationEvents.Handlers;

public sealed class AvailabilityUpsertedIntegrationEventHandler
    : IIntegrationEventHandler<AvailabilityUpsertedIntegrationEvent>
{
    private readonly ILogger<AvailabilityUpsertedIntegrationEventHandler> _logger;
    private readonly IMongoCollection<AccommodationDocument> _collection;

    public AvailabilityUpsertedIntegrationEventHandler(
        ILogger<AvailabilityUpsertedIntegrationEventHandler> logger,
        IMongoCollection<AccommodationDocument> collection)
    {
        _logger = logger;
        _collection = collection;
    }

    public async Task Handle(AvailabilityUpsertedIntegrationEvent @event, CancellationToken ct)
    {
        var filterExisting = Builders<AccommodationDocument>.Filter.And(
            Builders<AccommodationDocument>.Filter.Eq(x => x.Id, @event.AccommodationId),
            Builders<AccommodationDocument>.Filter.ElemMatch(
                x => x.Availabilities,
                a => a.Id == @event.AvailabilityId) 
        );

        var updateExisting = Builders<AccommodationDocument>.Update
            .Set("Availabilities.$.StartDate", @event.StartDate)
            .Set("Availabilities.$.EndDate", @event.EndDate)
            .Set("Availabilities.$.Price", @event.Price);

        var updateResult = await _collection.UpdateOneAsync(filterExisting, updateExisting, cancellationToken: ct);

        if (updateResult.MatchedCount > 0)
        {
            _logger.LogInformation(
                "Updated availability. AccommodationId={AccommodationId}, AvailabilityId={AvailabilityId}",
                @event.AccommodationId, @event.AvailabilityId);
            return;
        }

        var filterAccommodation = Builders<AccommodationDocument>.Filter.Eq(x => x.Id, @event.AccommodationId);

        var newAvailability = new AvailabilityDocument
        {
            Id = @event.AvailabilityId,
            StartDate = @event.StartDate,
            EndDate = @event.EndDate,
            Price = @event.Price
        };

        var pushUpdate = Builders<AccommodationDocument>.Update.Push(x => x.Availabilities, newAvailability);

        var pushResult = await _collection.UpdateOneAsync(filterAccommodation, pushUpdate, cancellationToken: ct);

        if (pushResult.MatchedCount == 0)
        {
            _logger.LogWarning(
                "Accommodation document not found in Mongo. Cannot upsert availability. AccommodationId={AccommodationId}, AvailabilityId={AvailabilityId}",
                @event.AccommodationId, @event.AvailabilityId);
            return;
        }

        _logger.LogInformation(
            "Inserted availability. AccommodationId={AccommodationId}, AvailabilityId={AvailabilityId}",
            @event.AccommodationId, @event.AvailabilityId);
    }
}
