namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Installation;
    using Microsoft.Extensions.Hosting;

    class NServiceBusInstallerService : IHostedService
    {
        IServiceProvider serviceProvider;
        IHostApplicationLifetime applicationLifetime;
        InstallerWithExternallyManagedContainer installer;

        public NServiceBusInstallerService(InstallerWithExternallyManagedContainer installer, IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider)
        {
            this.installer = installer;
            this.applicationLifetime = applicationLifetime;
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await installer.Run(serviceProvider, cancellationToken).ConfigureAwait(false);
            // immediately stop the application
            // This will continue to execute other installers but will update the cancellation token
            applicationLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
    class InstallerMessageSession : IMessageSession
    {
#pragma warning disable PS0018 // A task-returning method should have a CancellationToken parameter unless it has a parameter implementing ICancellableContext
        Task ThrowException() =>
            throw new InvalidOperationException(
                "Attempt to use the `IMessageSession` API while the endpoint runs in installation mode.");
#pragma warning restore PS0018 // A task-returning method should have a CancellationToken parameter unless it has a parameter implementing ICancellableContext

        public Task Send(object message, SendOptions sendOptions,
            CancellationToken cancellationToken = new CancellationToken()) =>
            ThrowException();

        public Task Send<T>(Action<T> messageConstructor, SendOptions sendOptions,
            CancellationToken cancellationToken = new CancellationToken()) =>
            ThrowException();

        public Task Publish(object message, PublishOptions publishOptions,
            CancellationToken cancellationToken = new CancellationToken()) =>
            ThrowException();

        public Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions,
            CancellationToken cancellationToken = new CancellationToken()) =>
            ThrowException();

        public Task Subscribe(Type eventType, SubscribeOptions subscribeOptions,
            CancellationToken cancellationToken = new CancellationToken()) =>
            ThrowException();

        public Task Unsubscribe(Type eventType, UnsubscribeOptions unsubscribeOptions,
            CancellationToken cancellationToken = new CancellationToken()) =>
            ThrowException();
    }
}