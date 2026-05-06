namespace AcceptanceTests
{
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

            var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
            endpointConfiguration.SendOnly();
            endpointConfiguration.UseTransport(new LearningTransport());
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

#pragma warning disable CS0618 // Type or member is obsolete
            hostBuilder.UseNServiceBus(endpointConfiguration);
#pragma warning restore CS0618 // Type or member is obsolete

            hostBuilder.Services.AddHostedService<HostedServiceThatAccessSessionInStart>();

            var host = hostBuilder.Build();
            await host.StartAsync();
            await host.StopAsync();
        }

        [Test]
        public async Task Should_be_available_when_configured_before_NServiceBus()
        {
            var hostBuilder = Host.CreateApplicationBuilder();
            hostBuilder.Services.AddHostedService<HostedServiceThatAccessSessionInStart>();

            var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
            endpointConfiguration.SendOnly();
            endpointConfiguration.UseTransport(new LearningTransport());
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

#pragma warning disable CS0618 // Type or member is obsolete
            hostBuilder.UseNServiceBus(endpointConfiguration);
#pragma warning restore CS0618 // Type or member is obsolete

            var host = hostBuilder.Build();
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
