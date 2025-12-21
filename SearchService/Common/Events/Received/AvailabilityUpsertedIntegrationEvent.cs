namespace SearchService.Common.Events.Received
{
    public record AvailabilityUpsertedIntegrationEvent(Guid AccommodationId, Guid AvailabilityId, DateTimeOffset StartDate, DateTimeOffset EndDate, decimal Price) : IIntegrationEvent;
}
