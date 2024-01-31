namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

    class EndpointStarter(
        IStartableEndpointWithExternallyManagedContainer startableEndpoint,
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory,
        DeferredLoggerFactory deferredLoggerFactory)
    {
        public async ValueTask<IEndpointInstance> GetOrStart(CancellationToken cancellationToken = default)
        {
            if (endpoint != null)
            {
                return endpoint;
            }

            await startSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (endpoint != null)
                {
                    return endpoint;
                }

                LogManager.UseFactory(new LoggerFactory(loggerFactory));
                deferredLoggerFactory.FlushAll(loggerFactory);

                endpoint = await startableEndpoint.Start(serviceProvider, cancellationToken)
                    .ConfigureAwait(false);

                return endpoint;
            }
            finally
            {
                startSemaphore.Release();
            }
        }

        readonly SemaphoreSlim startSemaphore = new(1, 1);

        IEndpointInstance endpoint;
    }
}