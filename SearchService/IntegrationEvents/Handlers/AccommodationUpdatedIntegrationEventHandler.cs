using MongoDB.Driver;
using SearchService.Common.Events;
using SearchService.Common.Events.Received;
using SearchService.Documents;
using SearchService.DTO;

namespace SearchService.IntegrationEvents.Handlers;

public class AccommodationUpdatedIntegrationEventHandler(
    IMongoCollection<AccommodationDocument> collection,
    ILogger<AccommodationUpdatedIntegrationEventHandler> logger)
    : IIntegrationEventHandler<AccommodationUpdatedIntegrationEvent>
{
    public async Task Handle(AccommodationUpdatedIntegrationEvent @event, CancellationToken ct)
    {
        var doc = MapToDocument(@event);

        await UpsertAsync(doc, ct);

        logger.LogInformation(
            "Upserted accommodation doc. AccommodationId={AccommodationId}",
            @event.AccommodationId);
    }

    private static AccommodationDocument MapToDocument(AccommodationUpdatedIntegrationEvent @event)
        => new()
        {
            Id = @event.AccommodationId,
            Name = @event.Name,
            PriceType = @event.PriceType,
            MinGuests = @event.MinGuest,
            MaxGuests = @event.MaxGuest,
            Location = MapLocation(@event.Location),
            Amenities = MapAmenities(@event.Amenities),
            Availabilities = [],
            Reservations = []
        };

    private static LocationDocument? MapLocation(LocationDTO? location)
        => location is null
            ? null
            : new LocationDocument
            {
                Address = location.Address,
                City = location.City,
                Country = location.Country,
                PostalCode = location.PostalCode
            };

    private static List<AmenityDocument> MapAmenities(List<AmenityDTO> amenities)
        => amenities
            .Select(a => new AmenityDocument
            {
                Name = a.Name,
                Description = a.Description
            })
            .ToList();

    private async Task UpsertAsync(AccommodationDocument doc, CancellationToken ct)
    {
        var filter = Builders<AccommodationDocument>.Filter.Eq(x => x.Id, doc.Id);
        var existing = await collection.Find(filter).FirstOrDefaultAsync(ct);

        if (existing is not null)
        {
            doc.Availabilities = existing.Availabilities;
            doc.Reservations = existing.Reservations;
        }
        await collection.ReplaceOneAsync(
            filter,
            doc,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }
}