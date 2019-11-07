namespace NServiceBus
{
    using Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Extension methods to configure an IServiceCollection for NServiceBus.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds NServiceBus, IMessageSession, and related services to the IServiceCollection.
        /// </summary>
        /// <remarks>
        /// Use this extension method with WebHost only.
        /// </remarks>
        public static IServiceCollection AddNServiceBus(this IServiceCollection services, EndpointConfiguration configuration)
        {
            var startableEndpoint = EndpointWithExternallyManagedContainer.Create(configuration, new ServiceCollectionAdapter(services));
            var hostedService = new NServiceBusHostedService(startableEndpoint);

            services.AddSingleton<IMessageSession>(_ => new WebHostCompatibleMessageSession(hostedService));
            services.AddSingleton<IHostedService>(serviceProvider =>
            {
                hostedService.UseServiceProvider(serviceProvider);
                return hostedService;
            });

            return services;
        }
    }
}