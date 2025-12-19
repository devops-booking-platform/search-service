using SearchService.Common.Events;

namespace SearchService.IntegrationEvents.Handlers
{
    public class ReservationApprovedEventHandler
    : IIntegrationEventHandler<ReservationApprovedEvent>
    {
        private readonly ILogger<ReservationApprovedEventHandler> _logger;

        public ReservationApprovedEventHandler(ILogger<ReservationApprovedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(ReservationApprovedEvent @event, CancellationToken ct)
        {
            _logger.LogInformation("Handling ReservationApprovedEvent");
            await Task.CompletedTask;
        }
    }
}
