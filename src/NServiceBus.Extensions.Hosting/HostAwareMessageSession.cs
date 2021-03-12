namespace NServiceBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    class HostAwareMessageSession : IMessageSession
    {
        public HostAwareMessageSession(Lazy<IMessageSession> messageSession)
        {
            this.messageSession = messageSession;
        }

        public Task Send(object message, SendOptions options, CancellationToken cancellationToken = default)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Send(message, options, cancellationToken);
        }

        public Task Send<T>(Action<T> messageConstructor, SendOptions options, CancellationToken cancellationToken = default)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Send(messageConstructor, options, cancellationToken);
        }

        public Task Publish(object message, PublishOptions options, CancellationToken cancellationToken = default)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Publish(message, options, cancellationToken);
        }

        public Task Publish<T>(Action<T> messageConstructor, PublishOptions options, CancellationToken cancellationToken = default)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Publish(messageConstructor, options, cancellationToken);
        }

        public Task Subscribe(Type eventType, SubscribeOptions options, CancellationToken cancellationToken = default)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Subscribe(eventType, options, cancellationToken);
        }

        public Task Unsubscribe(Type eventType, UnsubscribeOptions options, CancellationToken cancellationToken = default)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Unsubscribe(eventType, options, cancellationToken);
        }

        public void MarkReadyForUse()
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

        bool isReadyForUse;
        Lazy<IMessageSession> messageSession;
    }
}
