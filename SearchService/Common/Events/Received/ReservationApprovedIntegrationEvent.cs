namespace SearchService.Common.Events.Received
{
    public record ReservationApprovedIntegrationEvent(Guid AccommodationId, Guid ReservationId, DateOnly StartDate, DateOnly EndDate) : IIntegrationEvent;
}
