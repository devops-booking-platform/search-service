using SearchService.Common.Events;

namespace SearchService.IntegrationEvents.Handlers
{
    public class AvailabilityCreatedEventHandler
    : IIntegrationEventHandler<AvailabilityCreatedEvent>
    {
        private readonly ILogger<AvailabilityCreatedEventHandler> _logger;

        public AvailabilityCreatedEventHandler(ILogger<AvailabilityCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(AvailabilityCreatedEvent @event, CancellationToken ct)
        {
            _logger.LogInformation("Handling AvailabilityCreatedEvent");
            await Task.CompletedTask;
        }
    }
}
