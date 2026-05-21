namespace AcceptanceTests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NServiceBus;
    using NUnit.Framework;

    [TestFixture]
    public class When_session_is_accessed_in_hosted_service_start_with_host_builder
    {
        [Test]
        public async Task Should_be_available_when_configured_after_NServiceBus()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var host = Host.CreateDefaultBuilder()
                .UseNServiceBus(hostBuilderContext =>
                {
                    var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
                    endpointConfiguration.SendOnly();
                    endpointConfiguration.UseTransport(new LearningTransport());
                    endpointConfiguration.UseSerialization<SystemJsonSerializer>();
                    return endpointConfiguration;
                })
#pragma warning restore CS0618 // Type or member is obsolete
                .ConfigureServices((ctx, serviceProvider) => serviceProvider.AddHostedService<HostedServiceThatAccessSessionInStart>())
                .Build();

            await host.StartAsync();
            await host.StopAsync();
        }

        [Test]
        public async Task Should_be_available_when_configured_beforeNServiceBus()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((ctx, serviceProvider) => serviceProvider.AddHostedService<HostedServiceThatAccessSessionInStart>())
                .UseNServiceBus(hostBuilderContext =>
                {
                    var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
                    endpointConfiguration.SendOnly();
                    endpointConfiguration.UseTransport(new LearningTransport());
                    endpointConfiguration.UseSerialization<SystemJsonSerializer>();
                    return endpointConfiguration;
                })
#pragma warning restore CS0618 // Type or member is obsolete
                .Build();

            await host.StartAsync();
        }

        class HostedServiceThatAccessSessionInStart(IMessageSession messageSession) : IHostedService
        {
            public Task StartAsync(CancellationToken cancellationToken) => messageSession.Publish<MyEvent>(cancellationToken);

            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

            class MyEvent : IEvent
            {
            }
        }
    }
}
