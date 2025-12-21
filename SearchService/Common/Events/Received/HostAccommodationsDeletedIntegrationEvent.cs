namespace SearchService.Common.Events.Received
{
    public record HostAccommodationsDeletedIntegrationEvent(IReadOnlyList<Guid> AccommodationIds) : IIntegrationEvent;
}
