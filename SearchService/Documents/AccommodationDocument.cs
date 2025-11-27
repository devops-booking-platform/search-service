using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SearchService.Documents
{
    public class AccommodationDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Location { get; set; } = default!;
        public int MinGuests { get; set; }
        public int MaxGuests { get; set; }
        public List<string> Amenities { get; set; } = new();
        public decimal BasePrice { get; set; }
    }
}
