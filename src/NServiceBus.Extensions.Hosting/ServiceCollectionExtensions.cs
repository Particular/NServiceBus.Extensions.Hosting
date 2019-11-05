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
        /// Use this extension method with WebHost only.
        /// For GenericHost or ASP.NET Core version 3 or above use <see cref="HostBuilderExtensions.UseNServiceBus"/>
        /// </remarks>
        public static IServiceCollection AddNServiceBus(this IServiceCollection services, EndpointConfiguration configuration)
        {
            var startableEndpoint = EndpointWithExternallyManagedContainer.Create(configuration, new ServiceCollectionAdapter(services));

            services.AddSingleton(_ => startableEndpoint.MessageSession.Value);
            services.AddSingleton<IHostedService>(serviceProvider => new NServiceBusHostedService(startableEndpoint, serviceProvider));

            return services;
        }
    }
}