namespace SearchService.Common.Events.Received
{
    public record AvailabilityUpsertedIntegrationEvent(Guid AccommodationId, Guid AvailabilityId, DateOnly StartDate, DateOnly EndDate, decimal Price) : IIntegrationEvent;
}
