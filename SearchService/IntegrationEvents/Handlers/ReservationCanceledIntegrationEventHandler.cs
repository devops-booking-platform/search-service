using MongoDB.Driver;
using SearchService.Common.Events;
using SearchService.Common.Events.Received;
using SearchService.Documents;

namespace SearchService.IntegrationEvents.Handlers;

public sealed class ReservationCanceledIntegrationEventHandler
    : IIntegrationEventHandler<ReservationCanceledIntegrationEvent>
{
    private readonly ILogger<ReservationCanceledIntegrationEventHandler> _logger;
    private readonly IMongoCollection<AccommodationDocument> _collection;

    public ReservationCanceledIntegrationEventHandler(
        ILogger<ReservationCanceledIntegrationEventHandler> logger,
        IMongoCollection<AccommodationDocument> collection)
    {
        _logger = logger;
        _collection = collection;
    }

    public async Task Handle(ReservationCanceledIntegrationEvent @event, CancellationToken ct)
    {
        var filter = Builders<AccommodationDocument>.Filter.Eq(x => x.Id, @event.AccommodationId);

        var update = Builders<AccommodationDocument>.Update.PullFilter(
            x => x.Reservations,
            r => r.Id == @event.ReservationId);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);

        _logger.LogInformation(
            "Removed reservation from Mongo. AccommodationId={AccommodationId}, ReservationId={ReservationId}, Matched={Matched}, Modified={Modified}",
            @event.AccommodationId, @event.ReservationId, result.MatchedCount, result.ModifiedCount);
    }
}
