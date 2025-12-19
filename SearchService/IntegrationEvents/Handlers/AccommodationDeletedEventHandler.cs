using SearchService.Common.Events;

namespace SearchService.IntegrationEvents.Handlers
{
    public class AccommodationDeletedEventHandler
    : IIntegrationEventHandler<AccommodationDeletedEvent>
    {
        private readonly ILogger<AccommodationDeletedEventHandler> _logger;

        public AccommodationDeletedEventHandler(ILogger<AccommodationDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(AccommodationDeletedEvent @event, CancellationToken ct)
        {
            _logger.LogInformation("Handling AccommodationDeletedEvent");
            await Task.CompletedTask;
        }
    }
}
