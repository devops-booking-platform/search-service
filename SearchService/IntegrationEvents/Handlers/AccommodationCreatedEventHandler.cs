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
            var location = @event.Location is null
            ? null
            : new LocationDocument
            {
                Address = @event.Location.Address,
                City = @event.Location.City,
                Country = @event.Location.Country,
                PostalCode = @event.Location.PostalCode
            };
            var amenities = @event.Amenities
            .Select(a => new AmenityDocument
            {
                Name = a.Name,
                Description = a.Description
            })
            .ToList();

            var doc = new AccommodationDocument
            {
                Id = @event.AccommodationId.ToString(),
                Name = @event.Name,
                MinGuests = @event.MinGuest,
                MaxGuests = @event.MaxGuest,
                Location = location,
                Amenities = amenities,
                Availabilities = new(),
                Reservations = new()
            };

            var filter = Builders<AccommodationDocument>.Filter.Eq(x => x.Id, doc.Id);

            await _collection.ReplaceOneAsync(
                filter,
                doc,
                new ReplaceOptions { IsUpsert = true },
                ct);

            _logger.LogInformation("Upserted accommodation doc. AccommodationId={AccommodationId}", @event.AccommodationId);
        }
    }
}
