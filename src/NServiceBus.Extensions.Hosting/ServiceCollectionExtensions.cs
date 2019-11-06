namespace NServiceBus.WebHost
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
        /// Use this extension method with the ASP.NET Core WebHost only.
        /// For GenericHost or ASP.NET Core version 3 or above use <see cref="HostBuilderExtensions.UseNServiceBus" />
        /// </remarks>
        public static IServiceCollection AddNServiceBus(this IServiceCollection services, EndpointConfiguration configuration)
        {
            var startableEndpoint = EndpointWithExternallyManagedContainer.Create(configuration, new ServiceCollectionAdapter(services));
            var hostedService = new NServiceBusHostedService(startableEndpoint);
            // When there is a chance that the application is hosted using the WebHost, do not use the Lazy property
            // on the startableEndpoint as an early resolve will lock it into throwing an exception for the rest of
            // the applications lifetime.
            services.AddTransient<IMessageSession>(_ => hostedService.Endpoint);
            services.AddSingleton<IHostedService>(serviceProvider =>
            {
                hostedService.Configure(serviceProvider);
                return hostedService;
            });

            return services;
        }
    }
}