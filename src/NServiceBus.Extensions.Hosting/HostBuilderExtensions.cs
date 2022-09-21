namespace NServiceBus
{
    using System;
    using Extensions.Hosting;
    using Installation;
    using Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
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

                if (ctx.Configuration.GetValue<string>("nservicebus") == "install")
                {
                    // clear out other hosted services that were registered before `UseNServiceBus`
                    // Note that this won't affect hosted services registered afterwards
                    serviceCollection.RemoveAll(typeof(IHostedService));

                    var installer =
                        Installer.CreateInstallerWithExternallyManagedContainer(endpointConfiguration,
                            serviceCollection);
                    serviceCollection.AddHostedService(serviceProvider =>
                        new NServiceBusInstallerService(
                            installer,
                            serviceProvider.GetRequiredService<IHostApplicationLifetime>(),
                            serviceProvider));

                    // register a message session to allow dependencies to be resolved (e.g. in other hosted services)
                    serviceCollection.AddSingleton<IMessageSession, InstallerMessageSession>();
                }
                else
                {
                    var startableEndpoint = EndpointWithExternallyManagedContainer.Create(endpointConfiguration, serviceCollection);

                    serviceCollection.AddSingleton(_ => new HostAwareMessageSession(startableEndpoint.MessageSession));
                    serviceCollection.AddSingleton<IMessageSession>(serviceProvider => serviceProvider.GetService<HostAwareMessageSession>());
                    serviceCollection.AddSingleton<IHostedService>(serviceProvider => new NServiceBusHostedService(
                        startableEndpoint,
                        serviceProvider,
                        serviceProvider.GetRequiredService<ILoggerFactory>(),
                        deferredLoggerFactory,
                        serviceProvider.GetRequiredService<HostAwareMessageSession>()));
                }
            });

            return hostBuilder;
        }

        const string HostBuilderExtensionInUse = "NServiceBus.Extension.Hosting.UseNServiceBus";
    }
}