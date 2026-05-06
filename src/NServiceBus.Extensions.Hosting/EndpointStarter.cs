namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;

    using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

    class EndpointStarter(
#pragma warning disable CS0618 // Type or member is obsolete
        IStartableEndpointWithExternallyManagedContainer startableEndpoint,
#pragma warning restore CS0618 // Type or member is obsolete
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory,
        DeferredLoggerFactory deferredLoggerFactory)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        public async ValueTask<IEndpointInstance> GetOrStart(CancellationToken cancellationToken = default)
#pragma warning restore CS0618 // Type or member is obsolete
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

#pragma warning disable CS0618 // Type or member is obsolete
                LogManager.UseFactory(new LoggerFactory(loggerFactory));
                deferredLoggerFactory.FlushAll(loggerFactory);

                endpoint = await startableEndpoint.Start(serviceProvider, cancellationToken).ConfigureAwait(false);
#pragma warning restore CS0618 // Type or member is obsolete

                return endpoint;
            }
            finally
            {
                startSemaphore.Release();
            }
        }

        readonly SemaphoreSlim startSemaphore = new(1, 1);

#pragma warning disable CS0618 // Type or member is obsolete
        IEndpointInstance endpoint;
#pragma warning restore CS0618 // Type or member is obsolete
    }
}