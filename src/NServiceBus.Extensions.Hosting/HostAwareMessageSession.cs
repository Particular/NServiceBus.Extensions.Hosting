namespace NServiceBus
{
    using System;
    using System.Threading.Tasks;

    class HostAwareMessageSession : IMessageSession
    {
        public HostAwareMessageSession(Lazy<IMessageSession> messageSession)
        {
            this.messageSession = messageSession;
        }

        public Task Send(object message, SendOptions options)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Send(message, options);
        }

        public Task Send<T>(Action<T> messageConstructor, SendOptions options)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Send(messageConstructor, options);
        }

        public Task Publish(object message, PublishOptions options)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Publish(message, options);
        }

        public Task Publish<T>(Action<T> messageConstructor, PublishOptions options)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Publish(messageConstructor, options);
        }

        public Task Subscribe(Type eventType, SubscribeOptions options)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Subscribe(eventType, options);
        }

        public Task Unsubscribe(Type eventType, UnsubscribeOptions options)
        {
            GuardAgainstTooEarlyUse();

            return messageSession.Value.Unsubscribe(eventType, options);
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

            throw new InvalidOperationException("The message session can't be used before the NServiceBus hosted service is started. Make sure that the call to UseNServiceBus() happens before configuring any hosted service or the asp web pipeline.");
        }

        bool isReadyForUse;
        Lazy<IMessageSession> messageSession;
    }
}