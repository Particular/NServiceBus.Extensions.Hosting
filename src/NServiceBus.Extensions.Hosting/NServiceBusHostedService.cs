namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

    class NServiceBusHostedService : IHostedService, IMessageSession
    {
        public NServiceBusHostedService(IEndpointConfigurationBuilder endpointConfigurationBuilder,
            IServiceCollection serviceCollection,
            ILoggerFactory loggerFactory,
            DeferredLoggerFactory deferredLoggerFactory)
        {
            this.serviceCollection = serviceCollection;
            this.endpointConfigurationBuilder = endpointConfigurationBuilder;
            this.loggerFactory = loggerFactory;
            this.deferredLoggerFactory = deferredLoggerFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            LogManager.UseFactory(new LoggerFactory(loggerFactory));
            deferredLoggerFactory.FlushAll(loggerFactory);

            var endpointConfiguration = await endpointConfigurationBuilder.Build()
                .ConfigureAwait(false);

            var startableEndpoint =
                EndpointWithExternallyManagedContainer.Create(endpointConfiguration, serviceCollection);

            serviceProvider = serviceCollection.BuildServiceProvider();

            endpoint = await startableEndpoint.Start(serviceProvider)
                .ConfigureAwait(false);

            MarkReadyForUse();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await endpoint.Stop().ConfigureAwait(false);
            await serviceProvider.DisposeAsync().ConfigureAwait(false);
        }

        ServiceProvider serviceProvider;
        readonly DeferredLoggerFactory deferredLoggerFactory;
        readonly ILoggerFactory loggerFactory;
        readonly IEndpointConfigurationBuilder endpointConfigurationBuilder;
        readonly IServiceCollection serviceCollection;

        IEndpointInstance endpoint;
        bool isReadyForUse;

        public Task Send(object message, SendOptions options)
        {
            GuardAgainstTooEarlyUse();

            return endpoint.Send(message, options);
        }

        public Task Send<T>(Action<T> messageConstructor, SendOptions options)
        {
            GuardAgainstTooEarlyUse();

            return endpoint.Send(messageConstructor, options);
        }

        public Task Publish(object message, PublishOptions options)
        {
            GuardAgainstTooEarlyUse();

            return endpoint.Publish(message, options);
        }

        public Task Publish<T>(Action<T> messageConstructor, PublishOptions options)
        {
            GuardAgainstTooEarlyUse();

            return endpoint.Publish(messageConstructor, options);
        }

        public Task Subscribe(Type eventType, SubscribeOptions options)
        {
            GuardAgainstTooEarlyUse();

            return endpoint.Subscribe(eventType, options);
        }

        public Task Unsubscribe(Type eventType, UnsubscribeOptions options)
        {
            GuardAgainstTooEarlyUse();

            return endpoint.Unsubscribe(eventType, options);
        }

        void MarkReadyForUse()
        {
            isReadyForUse = true;
        }

        void GuardAgainstTooEarlyUse()
        {
            if (isReadyForUse)
            {
                return;
            }

            throw new InvalidOperationException("The message session can't be used before NServiceBus is started. Place `UseNServiceBus()` on the host builder before registering any hosted service (e.g. `services.AddHostedService<HostedServiceAccessingTheSession>()`) or the web host configuration (e.g. `builder.ConfigureWebHostDefaults`) should hosted services or controllers require access to the session.");
        }
    }
}