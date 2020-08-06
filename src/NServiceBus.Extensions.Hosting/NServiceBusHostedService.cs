namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.Extensions.Hosting;
    using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

    class NServiceBusHostedService : IHostedService
    {
        public NServiceBusHostedService(IStartableEndpointWithExternallyManagedContainer startableEndpoint, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, DeferredLoggerFactory deferredLoggerFactory)
        {
            this.loggerFactory = loggerFactory;
            this.deferredLoggerFactory = deferredLoggerFactory;
            this.startableEndpoint = startableEndpoint;
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            LogManager.UseFactory(new LoggerFactory(loggerFactory));
            deferredLoggerFactory.FlushAll(loggerFactory);

            endpoint = await startableEndpoint.Start(new ServiceProviderAdapter(serviceProvider))
                .ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return endpoint.Stop();
        }

        readonly IStartableEndpointWithExternallyManagedContainer startableEndpoint;
        readonly IServiceProvider serviceProvider;
        readonly DeferredLoggerFactory deferredLoggerFactory;
        readonly ILoggerFactory loggerFactory;

        IEndpointInstance endpoint;
    }
}