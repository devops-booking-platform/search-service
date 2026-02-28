using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SearchService.Documents
{
    public class AvailabilityDocument
    {
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal Price { get; set; }
    }
}
