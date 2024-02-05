namespace NServiceBus.Extensions.Hosting
{
    using Microsoft.Extensions.DependencyInjection;

    using MicrosoftILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

    static class ServiceCollectionExtensions
    {
        public static void AddNServiceBus(this IServiceCollection services, EndpointConfiguration endpointConfiguration, DeferredLoggerFactory loggerFactory)
        {
            var startableEndpoint = EndpointWithExternallyManagedContainer.Create(endpointConfiguration, services);

            services.AddSingleton(serviceProvider => new EndpointStarter(
                startableEndpoint,
                serviceProvider,
                serviceProvider.GetRequiredService<MicrosoftILoggerFactory>(),
                loggerFactory));

            services.AddSingleton(serviceProvider => new HostAwareMessageSession(serviceProvider.GetRequiredService<EndpointStarter>()));
            services.AddSingleton<IMessageSession>(serviceProvider => serviceProvider.GetRequiredService<HostAwareMessageSession>());
            services.AddHostedService(serviceProvider => new NServiceBusHostedService(serviceProvider.GetRequiredService<EndpointStarter>()));
        }
    }
}