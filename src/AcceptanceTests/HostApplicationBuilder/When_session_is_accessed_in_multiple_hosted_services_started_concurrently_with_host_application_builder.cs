namespace AcceptanceTests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NServiceBus;
    using NUnit.Framework;

    [TestFixture]
    public class When_session_is_accessed_in_multiple_hosted_services_started_concurrently_with_host_application_builder
    {
        [Test]
        public async Task Should_be_available_when_configured_after_NServiceBus()
        {
            var hostBuilder = Host.CreateApplicationBuilder();
            hostBuilder.Services.Configure<HostOptions>(options =>
            {
                options.ServicesStartConcurrently = true;
                options.ServicesStopConcurrently = true;
            });
            var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
            endpointConfiguration.SendOnly();
            endpointConfiguration.UseTransport(new LearningTransport());
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            hostBuilder.UseNServiceBus(endpointConfiguration);

            hostBuilder.Services.AddHostedService<FirstHostedService>();
            hostBuilder.Services.AddHostedService<SecondHostedService>();

            var host = hostBuilder.Build();
            await host.StartAsync();
            await host.StopAsync();
        }

        [Test]
        public async Task Should_be_available_when_configured_before_NServiceBus()
        {
            var hostBuilder = Host.CreateApplicationBuilder();
            hostBuilder.Services.Configure<HostOptions>(options =>
            {
                options.ServicesStartConcurrently = true;
                options.ServicesStopConcurrently = true;
            });

            hostBuilder.Services.AddHostedService<FirstHostedService>();
            hostBuilder.Services.AddHostedService<SecondHostedService>();

            var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
            endpointConfiguration.SendOnly();
            endpointConfiguration.UseTransport(new LearningTransport());
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            hostBuilder.UseNServiceBus(endpointConfiguration);

            var host = hostBuilder.Build();
            await host.StartAsync();
        }

        class FirstHostedService(IMessageSession messageSession) : IHostedService
        {
            public Task StartAsync(CancellationToken cancellationToken) => messageSession.Publish<MyEvent>(cancellationToken);

            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        }

        class SecondHostedService(IMessageSession messageSession) : IHostedService
        {
            public Task StartAsync(CancellationToken cancellationToken) => messageSession.Publish<MyEvent>(cancellationToken);

            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        }

        class MyEvent : IEvent;
    }
}
