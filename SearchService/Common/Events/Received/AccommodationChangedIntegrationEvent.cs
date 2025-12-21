using SearchService.DTO;

namespace SearchService.Common.Events.Received
{
    public record AccommodationChangedIntegrationEvent(Guid AccommodationId, string Name, int MaxGuest, int MinGuest, List<AmenityDTO> Amenities, LocationDTO? Location) : IIntegrationEvent;
}
