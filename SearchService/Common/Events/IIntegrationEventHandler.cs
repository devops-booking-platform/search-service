namespace SearchService.Common.Events
{
    public interface IIntegrationEventHandler<in T>
    {
        Task Handle(T @event, CancellationToken ct);
    }
}
