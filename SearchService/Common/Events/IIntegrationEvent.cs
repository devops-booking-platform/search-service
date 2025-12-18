using SearchService.DTO;

namespace SearchService.Common.Events
{
    public interface IIntegrationEvent { }
    public record ReservationApprovedEvent(Guid AccommodationId, Guid ReservationId, DateTimeOffset StartDate, DateTimeOffset EndDate) : IIntegrationEvent;
    public record ReservationCanceledEvent(Guid AccommodationId, Guid ReservationId) : IIntegrationEvent;
    public record AvailabilityCreatedEvent(Guid AccommodationId, Guid AvailabilityId, DateTimeOffset StartDate, DateTimeOffset EndDate, decimal Price) : IIntegrationEvent;
    public record AccommodationCreatedEvent(Guid AccommodationId, string Name, int MaxGuest, int MinGuest, List<AmenityDTO> Amenities, LocationDTO Location) : IIntegrationEvent;
    public record AccommodationDeletedEvent(Guid AccommodationId) : IIntegrationEvent;
}
