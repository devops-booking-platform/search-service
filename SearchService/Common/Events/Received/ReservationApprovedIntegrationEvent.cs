namespace SearchService.Common.Events.Received
{
    public record ReservationApprovedIntegrationEvent(Guid AccommodationId, Guid ReservationId, DateTimeOffset StartDate, DateTimeOffset EndDate) : IIntegrationEvent;
}
