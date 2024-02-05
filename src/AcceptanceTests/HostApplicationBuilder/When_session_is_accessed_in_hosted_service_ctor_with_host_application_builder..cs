namespace AcceptanceTests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NServiceBus;
    using NUnit.Framework;

    [TestFixture]
    public class When_session_is_accessed_in_hosted_service_ctor_with_host_application_builder
    {
        [Test]
        public async Task Should_be_available()
        {
            var hostBuilder = Host.CreateApplicationBuilder();

            var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
            endpointConfiguration.SendOnly();
            endpointConfiguration.UseTransport(new LearningTransport());
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            hostBuilder.UseNServiceBus(endpointConfiguration);

            hostBuilder.Services.AddHostedService<HostedServiceThatAccessSessionInCtor>();

            var host = hostBuilder.Build();

            await host.StartAsync();
        }

        class HostedServiceThatAccessSessionInCtor : IHostedService
        {
            public HostedServiceThatAccessSessionInCtor(IMessageSession messageSession)
                // Yes this is weird but would work
                => messageSession.Publish<MyEvent>().GetAwaiter().GetResult();

            public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

            class MyEvent : IEvent
            {
            }
        }
    }
}
