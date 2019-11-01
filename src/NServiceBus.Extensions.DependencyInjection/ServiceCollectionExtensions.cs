namespace NServiceBus.Extensions.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NServiceBus;

    /// <summary>
    /// Extensions methods to configure an IServiceCollection for NServiceBus.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds NServiceBus, IMessageSession, and related services to the IServiceCollection.
        /// </summary>
        public static IServiceCollection AddNServiceBus(this IServiceCollection services, EndpointConfiguration configuration)
        {
            var startableEndpoint = EndpointWithExternallyManagedContainer.Create(configuration, new ServiceCollectionAdapter(services));

            services.AddSingleton(_ => startableEndpoint.MessageSession.Value);
            services.AddSingleton<IHostedService>(serviceProvider => new NServiceBusHostedService(startableEndpoint, serviceProvider));

            return services;
        }
    }
}