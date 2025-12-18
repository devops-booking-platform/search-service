using MongoDB.Driver;
using SearchService.Common.Events;
using SearchService.Documents;
using SearchService.DTO;

namespace SearchService.IntegrationEvents.Handlers
{
    public class AccommodationCreatedEventHandler
    : IIntegrationEventHandler<AccommodationCreatedEvent>
    {
        private readonly IMongoCollection<AccommodationDocument> _collection;
        private readonly ILogger<AccommodationCreatedEventHandler> _logger;

        public AccommodationCreatedEventHandler(IMongoCollection<AccommodationDocument> collection,
        ILogger<AccommodationCreatedEventHandler> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        public async Task Handle(AccommodationCreatedEvent @event, CancellationToken ct)
        {
            var doc = MapToDocument(@event);

            await UpsertAsync(doc, ct);

            _logger.LogInformation(
                "Upserted accommodation doc. AccommodationId={AccommodationId}",
                @event.AccommodationId);
        }

        private static AccommodationDocument MapToDocument(AccommodationCreatedEvent @event)
            => new()
            {
                Id = @event.AccommodationId.ToString(),
                Name = @event.Name,
                MinGuests = @event.MinGuest,
                MaxGuests = @event.MaxGuest,
                Location = MapLocation(@event.Location),
                Amenities = MapAmenities(@event.Amenities),
                Availabilities = new(),
                Reservations = new()
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

        private Task UpsertAsync(AccommodationDocument doc, CancellationToken ct)
        {
            var filter = Builders<AccommodationDocument>.Filter.Eq(x => x.Id, doc.Id);

            return _collection.ReplaceOneAsync(
                filter,
                doc,
                new ReplaceOptions { IsUpsert = true },
                ct);
        }
    }
}
