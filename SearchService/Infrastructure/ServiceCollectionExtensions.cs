using SearchService.Common.Events;
using SearchService.Common.Events.Received;
using SearchService.IntegrationEvents.Handlers;
using SearchService.Services.Interfaces;

namespace SearchService.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSearchServiceDependencies(this IServiceCollection services)
        {
            services.AddScoped<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
            services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<AccommodationCreatedIntegrationEvent>>();
            services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<AccommodationUpdatedIntegrationEvent>>();
            services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<AvailabilityUpsertedIntegrationEvent>>();
            services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<ReservationApprovedIntegrationEvent>>();
            services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<ReservationCanceledIntegrationEvent>>();
            services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<HostAccommodationsDeletedIntegrationEvent>>();

            services.AddScoped<IIntegrationEventHandler<AccommodationCreatedIntegrationEvent>, AccommodationCreatedIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<AccommodationUpdatedIntegrationEvent>, AccommodationUpdatedIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<AvailabilityUpsertedIntegrationEvent>, AvailabilityUpsertedIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<ReservationApprovedIntegrationEvent>, ReservationApprovedIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<ReservationCanceledIntegrationEvent>, ReservationCanceledIntegrationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<HostAccommodationsDeletedIntegrationEvent>, HostAccommodationsDeletedIntegrationEventHandler>();
            services.AddScoped<ISearchService, SearchService.Services.SearchService>();
            services.AddHostedService<IntegrationEventsSubscriber>();
            return services;
        }
    }
}
