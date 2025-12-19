using SearchService.Common.Events;
using SearchService.IntegrationEvents.Handlers;

namespace SearchService.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSearchServiceDependencies(this IServiceCollection services)
        {
            services.AddScoped<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
            services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<AccommodationCreatedEvent>>();
            services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<AccommodationDeletedEvent>>();
            services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<AvailabilityCreatedEvent>>();
            services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<ReservationApprovedEvent>>();
            services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<ReservationCanceledEvent>>();
            services.AddScoped<IIntegrationEventHandler<AccommodationCreatedEvent>, AccommodationCreatedEventHandler>();
            services.AddScoped<IIntegrationEventHandler<AccommodationDeletedEvent>, AccommodationDeletedEventHandler>();
            services.AddScoped<IIntegrationEventHandler<AvailabilityCreatedEvent>, AvailabilityCreatedEventHandler>();
            services.AddScoped<IIntegrationEventHandler<ReservationApprovedEvent>, ReservationApprovedEventHandler>();
            services.AddScoped<IIntegrationEventHandler<ReservationCanceledEvent>, ReservationCanceledEventHandler>();
            services.AddHostedService<IntegrationEventsSubscriber>();
            return services;
        }
    }
}
