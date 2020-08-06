namespace NServiceBus
{
    using System;
    using System.Linq;
    using Extensions.Hosting;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

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
            var deferredLoggerFactory = new DeferredLoggerFactory();
            LogManager.UseFactory(deferredLoggerFactory);

            hostBuilder.ConfigureServices((ctx, serviceCollection) =>
            {
                var endpointConfiguration = endpointConfigurationBuilder(ctx);
                var startableEndpoint = EndpointWithExternallyManagedContainer.Create(endpointConfiguration, new ServiceCollectionAdapter(serviceCollection));

                if (serviceCollection.Any(x =>
                    x.ServiceType == typeof(IHostedService) &&
                    x.ImplementationFactory != null &&
                    x.ImplementationFactory.Method.ReturnType == typeof(NServiceBusHostedService) &&
                    x.Lifetime == ServiceLifetime.Singleton))
                {
                    throw new InvalidOperationException(
                        "`UseNServiceBus` can only be used once on the same host instance because subsequent calls would override each other. For multi-endpoint hosting scenarios consult our documentation page.");
                }

                serviceCollection.AddSingleton(_ => startableEndpoint.MessageSession.Value);
                serviceCollection.AddSingleton<IHostedService, NServiceBusHostedService>(Factory);

                NServiceBusHostedService Factory(IServiceProvider serviceProvider)
                {
                    return new NServiceBusHostedService(
                        startableEndpoint,
                        serviceProvider,
                        serviceProvider.GetRequiredService<ILoggerFactory>(),
                        deferredLoggerFactory);
                }
            });

            return hostBuilder;
        }
    }
}