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
            hostBuilder.ConfigureServices((ctx, serviceCollection) =>
            {
                serviceCollection.AddSingleton<IEndpointConfigurationBuilder>(_ => new UserActionEndpointConfigurationBuilder(endpointConfigurationBuilder, ctx));
            });

            return hostBuilder.UseNServiceBus();
        }

        /// <summary>
        /// Configures the host to start an NServiceBus endpoint configured by an implementation of <see cref="IEndpointConfigurationBuilder"/>.
        /// </summary>
        public static IHostBuilder UseNServiceBus(this IHostBuilder hostBuilder)
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

                serviceCollection.AddSingleton<IMessageSession>(serviceProvider => serviceProvider.GetService<NServiceBusHostedService>());
                serviceCollection.AddSingleton(serviceProvider => new NServiceBusHostedService(
                    serviceProvider.GetRequiredService<IEndpointConfigurationBuilder>(),
                    serviceCollection,
                    serviceProvider.GetRequiredService<ILoggerFactory>(),
                    deferredLoggerFactory));
                serviceCollection.AddSingleton<IHostedService>(serviceProvider => serviceProvider.GetService<NServiceBusHostedService>());
            });

            return hostBuilder;
        }

        const string HostBuilderExtensionInUse = "NServiceBus.Extension.Hosting.UseNServiceBus";
    }
}