using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SearchService.Documents
{
    public class ReservationDocument
    {
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
