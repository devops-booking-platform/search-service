namespace SearchService.Common.Events.Received
{
    public record ReservationCanceledIntegrationEvent(Guid AccommodationId, Guid ReservationId) : IIntegrationEvent;
}
