namespace NServiceBus
{
    using System;
    using Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using ObjectBuilder;

    /// <summary>
    /// Extension methods to configure NServiceBus for the .NET Core generic host.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configures the host to start an NServiceBus endpoint.
        /// </summary>
        public static IHostBuilder UseNServiceBus(this IHostBuilder hostBuilder, Func<HostBuilderContext, EndpointConfiguration> endpointConfigurationBuilder)
        {
            return hostBuilder.UseNServiceBus<EndpointInstanceStarter>(endpointConfigurationBuilder);
        }

        /// <summary>
        /// Configure the host to start an NServiceBus endpoint using <typeparamref name="TEndpointInstanceStart" />.
        /// </summary>
        /// <typeparam name="TEndpointInstanceStart">Implementation of <see cref="IEndpointInstanceStarter" />.</typeparam>
        public static IHostBuilder UseNServiceBus<TEndpointInstanceStart>(this IHostBuilder hostBuilder, Func<HostBuilderContext, EndpointConfiguration> endpointConfigurationBuilder) where TEndpointInstanceStart : class, IEndpointInstanceStarter
        {
            hostBuilder.ConfigureServices((ctx, serviceCollection) =>
            {
                var endpointConfiguration = endpointConfigurationBuilder(ctx);
                var startableEndpoint = EndpointWithExternallyManagedContainer.Create(endpointConfiguration, new ServiceCollectionAdapter(serviceCollection));
                serviceCollection.AddSingleton<IBuilder>(serviceProvider => new ServiceProviderAdapter(serviceProvider));
                serviceCollection.AddSingleton(startableEndpoint);
                serviceCollection.AddSingleton<IEndpointInstanceStarter, TEndpointInstanceStart>();
                serviceCollection.AddSingleton(_ => startableEndpoint.MessageSession.Value);
                serviceCollection.AddSingleton<IHostedService, NServiceBusHostedService>();
            });

            return hostBuilder;
        }
    }
}