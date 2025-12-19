using SearchService.Common.Events;

namespace SearchService.IntegrationEvents.Handlers
{
    public class ReservationCanceledEventHandler
    : IIntegrationEventHandler<ReservationCanceledEvent>
    {
        private readonly ILogger<ReservationCanceledEventHandler> _logger;

        public ReservationCanceledEventHandler(ILogger<ReservationCanceledEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(ReservationCanceledEvent @event, CancellationToken ct)
        {
            _logger.LogInformation("Handling ReservationCanceledEvent");
            await Task.CompletedTask;
        }
    }
}
