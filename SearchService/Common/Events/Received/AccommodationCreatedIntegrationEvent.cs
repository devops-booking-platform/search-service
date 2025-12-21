using SearchService.DTO;
namespace SearchService.Common.Events.Received
{
    public record AccommodationCreatedIntegrationEvent(Guid AccommodationId, string Name, int MaxGuest, int MinGuest, List<AmenityDTO> Amenities, LocationDTO? Location) : IIntegrationEvent;
}
