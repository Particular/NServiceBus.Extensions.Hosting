namespace NServiceBus.Extensions.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NServiceBus;

    /// <summary>
    /// TODO: document
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// TODO: document
        /// </summary>
        public static IServiceCollection AddNServiceBus(this IServiceCollection services, EndpointConfiguration configuration)
        {
            var startableEndpoint = EndpointWithExternallyManagedContainer.Create(configuration, new ServiceCollectionAdapter(services));

            services.AddSingleton(_ => startableEndpoint.MessageSession.Value);
            services.AddSingleton<IHostedService>(serviceProvider => new NServiceBusService(startableEndpoint, serviceProvider));

            return services;
        }
    }
}