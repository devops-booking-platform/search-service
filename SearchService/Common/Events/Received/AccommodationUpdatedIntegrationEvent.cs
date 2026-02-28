using SearchService.DTO;
using SearchService.Enums;

namespace SearchService.Common.Events.Received;

public record AccommodationUpdatedIntegrationEvent(
    Guid AccommodationId,
    string Name,
    int MaxGuest,
    int MinGuest,
    List<AmenityDTO> Amenities,
    LocationDTO? Location,
    PriceType PriceType) : IIntegrationEvent;