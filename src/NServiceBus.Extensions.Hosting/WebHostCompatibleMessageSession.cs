namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Threading.Tasks;

    class WebHostCompatibleMessageSession : IMessageSession
    {
        NServiceBusHostedService hostedService;

        public WebHostCompatibleMessageSession(NServiceBusHostedService hostedService)
        {
            this.hostedService = hostedService;
        }

        public async Task Send(object message, SendOptions options)
        {
            var session = await hostedService.Endpoint.ConfigureAwait(true);
            await session.Send(message, options).ConfigureAwait(true);
        }

        public async Task Send<T>(Action<T> messageConstructor, SendOptions options)
        {
            var session = await hostedService.Endpoint.ConfigureAwait(true);
            await session.Send(messageConstructor, options).ConfigureAwait(true);
        }

        public async Task Publish(object message, PublishOptions options)
        {
            var session = await hostedService.Endpoint.ConfigureAwait(true);
            await session.Publish(message, options).ConfigureAwait(true);
        }

        public async Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
        {
            var session = await hostedService.Endpoint.ConfigureAwait(true);
            await session.Publish(messageConstructor, publishOptions).ConfigureAwait(true);
        }

        public async Task Subscribe(Type eventType, SubscribeOptions options)
        {
            var session = await hostedService.Endpoint.ConfigureAwait(true);
            await session.Subscribe(eventType, options).ConfigureAwait(true);
        }

        public async Task Unsubscribe(Type eventType, UnsubscribeOptions options)
        {
            var session = await hostedService.Endpoint.ConfigureAwait(true);
            await session.Unsubscribe(eventType, options).ConfigureAwait(true);
        }
    }
}