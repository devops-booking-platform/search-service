using MongoDB.Driver;
using SearchService.Common.Events;
using SearchService.Common.Events.Received;
using SearchService.Documents;

namespace SearchService.IntegrationEvents.Handlers;

public sealed class ReservationApprovedIntegrationEventHandler
    : IIntegrationEventHandler<ReservationApprovedIntegrationEvent>
{
    private readonly ILogger<ReservationApprovedIntegrationEventHandler> _logger;
    private readonly IMongoCollection<AccommodationDocument> _collection;

    public ReservationApprovedIntegrationEventHandler(
        ILogger<ReservationApprovedIntegrationEventHandler> logger,
        IMongoCollection<AccommodationDocument> collection)
    {
        _logger = logger;
        _collection = collection;
    }

    public async Task Handle(ReservationApprovedIntegrationEvent @event, CancellationToken ct)
    {
        var filterExisting = Builders<AccommodationDocument>.Filter.And(
            Builders<AccommodationDocument>.Filter.Eq(x => x.Id, @event.AccommodationId),
            Builders<AccommodationDocument>.Filter.ElemMatch(
                x => x.Reservations,
                r => r.Id == @event.ReservationId)
        );

        var updateExisting = Builders<AccommodationDocument>.Update
            .Set("Reservations.$.StartDate", @event.StartDate)
            .Set("Reservations.$.EndDate", @event.EndDate);

        var updateResult = await _collection.UpdateOneAsync(filterExisting, updateExisting, cancellationToken: ct);

        if (updateResult.MatchedCount > 0)
        {
            _logger.LogInformation(
                "Updated reservation in Mongo. AccommodationId={AccommodationId}, ReservationId={ReservationId}",
                @event.AccommodationId, @event.ReservationId);
            return;
        }

        var filterAccommodation = Builders<AccommodationDocument>.Filter.Eq(x => x.Id, @event.AccommodationId);

        var newReservation = new ReservationDocument
        {
            Id = @event.ReservationId,
            StartDate = @event.StartDate,
            EndDate = @event.EndDate
        };

        var pushUpdate = Builders<AccommodationDocument>.Update.Push(x => x.Reservations, newReservation);

        var pushResult = await _collection.UpdateOneAsync(filterAccommodation, pushUpdate, cancellationToken: ct);

        if (pushResult.MatchedCount == 0)
        {
            _logger.LogWarning(
                "Accommodation doc not found in Mongo for ReservationApproved. AccommodationId={AccommodationId}",
                @event.AccommodationId);
            return;
        }

        _logger.LogInformation(
            "Inserted reservation in Mongo. AccommodationId={AccommodationId}, ReservationId={ReservationId}",
            @event.AccommodationId, @event.ReservationId);
    }
}
