namespace NServiceBus
{
    using System;
    using Extensions.Hosting;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

    /// <summary>
    /// Extension methods to configure NServiceBus for the .NET hosted applications builder.
    /// </summary>
    public static class HostApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures the host application builder to start an NServiceBus endpoint.
        /// </summary>
        /// <returns></returns>
        public static IHostApplicationBuilder UseNServiceBus(this IHostApplicationBuilder builder, EndpointConfiguration endpointConfiguration)
        {
            var deferredLoggerFactory = new DeferredLoggerFactory();
            LogManager.UseFactory(deferredLoggerFactory);

            if (builder.Properties.ContainsKey(HostBuilderExtensionInUse))
            {
                throw new InvalidOperationException(
                    "`UseNServiceBus` can only be used once on the same host instance because subsequent calls would override each other. For multi-endpoint hosting scenarios consult our documentation page.");
            }

            builder.Properties.Add(HostBuilderExtensionInUse, null);

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