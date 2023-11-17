﻿namespace AcceptanceTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NServiceBus;
    using NUnit.Framework;

    [TestFixture]
    public class When_session_is_accessed_in_hosted_service_start_with_host_application_builder
    {
        [Test]
        public async Task Should_be_available_when_configured_after_NServiceBus()
        {
            var hostBuilder = Host.CreateApplicationBuilder();
            hostBuilder.UseNServiceBus(() =>
            {
                var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
                endpointConfiguration.SendOnly();
                endpointConfiguration.UseTransport(new LearningTransport());
                return endpointConfiguration;
            });

            hostBuilder.Services.AddHostedService<HostedServiceThatAccessSessionInStart>();

            var host = hostBuilder.Build();
            await host.StartAsync();
            await host.StopAsync();
        }

        [Test]
        public void Should_throw_when_hosted_service_is_configured_before_NServiceBus()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var hostBuilder = Host.CreateApplicationBuilder();
                hostBuilder.Services.AddHostedService<HostedServiceThatAccessSessionInStart>();

                hostBuilder.UseNServiceBus(() =>
                {
                    var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
                    endpointConfiguration.SendOnly();
                    endpointConfiguration.UseTransport(new LearningTransport());
                    return endpointConfiguration;
                });

                var host = hostBuilder.Build();
                await host.StartAsync();
            });

            StringAssert.Contains("The message session can't be used before NServiceBus is started", ex.Message);
        }

        class HostedServiceThatAccessSessionInStart : IHostedService
        {
            public HostedServiceThatAccessSessionInStart(IMessageSession messageSession)
            {
                this.messageSession = messageSession;
            }
            public Task StartAsync(CancellationToken cancellationToken)
            {
                return messageSession.Publish<MyEvent>(cancellationToken);
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            class MyEvent : IEvent
            {
            }

            readonly IMessageSession messageSession;
        }
    }
}