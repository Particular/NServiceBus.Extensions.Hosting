namespace NServiceBus.WebHost
{
    using System;
    using System.Threading;
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
            var hostedService = new NServiceBusHostedService(startableEndpoint);

            services.AddSingleton<IHostedService>(serviceProvider =>
            {
                hostedService.UseServiceProvider(serviceProvider);
                return hostedService;
            });
            services.AddSingleton<IMessageSession>(_ =>
            {
                if (hostedService.Endpoint != null)
                {
                    return hostedService.Endpoint;
                }

                var timeout = TimeSpan.FromSeconds(30);
                if (SpinWait.SpinUntil(() => hostedService.Endpoint != null, timeout))
                {
                    return hostedService.Endpoint;
                }

                throw new TimeoutException($"Unable to resolve the message session within '{timeout:g}'. Verify that the endpoint was able to start.");
            });

            return services;
        }
    }
}