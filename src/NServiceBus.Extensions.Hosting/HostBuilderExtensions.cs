﻿namespace NServiceBus
{
    using System;
    using Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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
                var endpointConfiguration = endpointConfigurationBuilder(ctx);
                var startableEndpoint = EndpointWithExternallyManagedContainer.Create(
                    endpointConfiguration, 
                    new ServiceCollectionAdapter(serviceCollection));

                serviceCollection.AddSingleton(_ => startableEndpoint.MessageSession.Value);
                serviceCollection.AddSingleton<IHostedService>(serviceProvider =>
                {
                    var hostedService = new NServiceBusHostedService(startableEndpoint);
                    hostedService.Configure(serviceProvider);
                    return hostedService;
                });
            });

            return hostBuilder;
        }
    }
}