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
        public NServiceBusHostedService(IStartableEndpointWithExternallyManagedContainer startableEndpoint,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            DeferredLoggerFactory deferredLoggerFactory,
            HostAwareMessageSession hostAwareMessageSession)
        {
            this.loggerFactory = loggerFactory;
            this.deferredLoggerFactory = deferredLoggerFactory;
            this.hostAwareMessageSession = hostAwareMessageSession;
            this.startableEndpoint = startableEndpoint;
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            LogManager.UseFactory(new LoggerFactory(loggerFactory));
            deferredLoggerFactory.FlushAll(loggerFactory);

            endpoint = await startableEndpoint.Start(serviceProvider, cancellationToken)
                .ConfigureAwait(false);

            hostAwareMessageSession.MarkReadyForUse();
        }

        public Task StopAsync(CancellationToken cancellationToken = default) =>
            endpoint?.Stop(cancellationToken) ?? Task.CompletedTask;

        readonly IStartableEndpointWithExternallyManagedContainer startableEndpoint;
        readonly IServiceProvider serviceProvider;
        readonly DeferredLoggerFactory deferredLoggerFactory;
        readonly HostAwareMessageSession hostAwareMessageSession;
        readonly ILoggerFactory loggerFactory;

        IEndpointInstance endpoint;
    }
}