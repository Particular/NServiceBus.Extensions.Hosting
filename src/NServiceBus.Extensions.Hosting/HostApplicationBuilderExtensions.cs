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
    public static class HostApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures the host to start an NServiceBus endpoint.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureEndpoint"></param>
        /// <returns></returns>
        public static IHostApplicationBuilder UseNServiceBus(this IHostApplicationBuilder builder, Func<EndpointConfiguration> configureEndpoint)
        {
            var deferredLoggerFactory = new DeferredLoggerFactory();
            LogManager.UseFactory(deferredLoggerFactory);

            if (builder.Properties.ContainsKey(HostBuilderExtensionInUse))
            {
                throw new InvalidOperationException(
                    "`UseNServiceBus` can only be used once on the same host instance because subsequent calls would override each other. For multi-endpoint hosting scenarios consult our documentation page.");
            }

            builder.Properties.Add(HostBuilderExtensionInUse, null);

            var endpointConfiguration = configureEndpoint();
            var startableEndpoint = EndpointWithExternallyManagedContainer.Create(endpointConfiguration, builder.Services);

            builder.Services.AddSingleton(_ => new HostAwareMessageSession(startableEndpoint.MessageSession));
            builder.Services.AddSingleton<IMessageSession>(serviceProvider => serviceProvider.GetRequiredService<HostAwareMessageSession>());
            builder.Services.AddHostedService(serviceProvider => new NServiceBusHostedService(
                startableEndpoint,
                serviceProvider,
                serviceProvider.GetRequiredService<ILoggerFactory>(),
                deferredLoggerFactory,
                serviceProvider.GetRequiredService<HostAwareMessageSession>()));

            return builder;
        }

        const string HostBuilderExtensionInUse = "NServiceBus.Extension.Hosting.UseNServiceBus";
    }
}