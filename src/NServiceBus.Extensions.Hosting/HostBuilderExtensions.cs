namespace NServiceBus
{
    using System;
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
                if (ctx.Properties.ContainsKey(HostBuilderExtensionInUse))
                {
                    throw new InvalidOperationException(
                        "`UseNServiceBus` can only be used once on the same host instance because subsequent calls would override each other. For multi-endpoint hosting scenarios consult our documentation page.");
                }

                ctx.Properties.Add(HostBuilderExtensionInUse, null);

                var endpointConfiguration = endpointConfigurationBuilder(ctx);
                var startableEndpoint = EndpointWithExternallyManagedContainer.Create(endpointConfiguration, new ServiceCollectionAdapter(serviceCollection));

                serviceCollection.AddSingleton(_ => new HostAwareMessageSession(startableEndpoint.MessageSession));
                serviceCollection.AddSingleton<IMessageSession>(serviceProvider => serviceProvider.GetService<HostAwareMessageSession>());
                serviceCollection.AddSingleton<IHostedService>(serviceProvider => new NServiceBusHostedService(
                    startableEndpoint,
                    serviceProvider,
                    serviceProvider.GetRequiredService<ILoggerFactory>(),
                    deferredLoggerFactory,
                    serviceProvider.GetRequiredService<HostAwareMessageSession>()));
            });

            return hostBuilder;
        }

        const string HostBuilderExtensionInUse = "NServiceBus.Extension.Hosting.UseNServiceBus";
    }
}