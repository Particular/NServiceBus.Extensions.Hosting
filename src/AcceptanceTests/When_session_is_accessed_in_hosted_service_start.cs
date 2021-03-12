namespace AcceptanceTests
{
    using Microsoft.Extensions.Hosting;
    using NUnit.Framework;
    using NServiceBus;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    [TestFixture]
    public class When_session_is_accessed_in_hosted_service_start
    {
        [Test]
        public async Task Should_be_available_when_configured_after_NServiceBus()
        {
            var host = Host.CreateDefaultBuilder()
                .UseNServiceBus(hostBuilderContext =>
                {
                    var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
                    endpointConfiguration.SendOnly();
                    endpointConfiguration.UseTransport(new LearningTransport());
                    return endpointConfiguration;
                })
                .ConfigureServices((ctx, serviceProvider) => serviceProvider.AddHostedService<HostedServiceThatAccessSessionInStart>())
                .Build();

            await host.StartAsync();
            await host.StopAsync();
        }

        [Test]
        public void Should_throw_when_hosted_service_is_configured_before_NServiceBus()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var host = Host.CreateDefaultBuilder()
                .ConfigureServices((ctx, serviceProvider) => serviceProvider.AddHostedService<HostedServiceThatAccessSessionInStart>())
                .UseNServiceBus(hostBuilderContext =>
                  {
                      var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
                      endpointConfiguration.SendOnly();
                      endpointConfiguration.UseTransport(new LearningTransport());
                      return endpointConfiguration;
                  })
                .Build();

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
