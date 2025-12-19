using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SearchService.Documents
{
    public class AccommodationDocument
    {
        [BsonId]

        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int MinGuests { get; set; }
        public int MaxGuests { get; set; }
        public List<AmenityDocument> Amenities { get; set; } = [];
        public List<AvailabilityDocument> Availabilities { get; set; } = [];
        public List<ReservationDocument> Reservations { get; set; } = [];
        public LocationDocument? Location { get; set; } = null!;
    }
}
