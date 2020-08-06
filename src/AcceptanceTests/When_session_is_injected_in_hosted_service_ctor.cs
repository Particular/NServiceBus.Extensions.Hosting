namespace AcceptanceTests
{
    using Microsoft.Extensions.Hosting;
    using NUnit.Framework;
    using NServiceBus;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;
    using System.Threading;

    [TestFixture]
    public class When_session_is_injected_in_hosted_service_ctor
    {
        [Test]
        public async Task Should_be_usable_in_start()
        {
            var host = Host.CreateDefaultBuilder()
            .UseNServiceBus(hostBuilderContext =>
            {
                var endpointConfiguration = new EndpointConfiguration("NSBRepro");
                endpointConfiguration.SendOnly();
                endpointConfiguration.UseTransport<LearningTransport>();
                return endpointConfiguration;
            })
            .ConfigureServices((ctx, serviceProvider) => serviceProvider.AddHostedService<HostedServiceThatInjectsSessionIntoCtor>()).Build();

            await host.StartAsync();
            await host.StopAsync();
        }

        class HostedServiceThatInjectsSessionIntoCtor : IHostedService
        {
            public HostedServiceThatInjectsSessionIntoCtor(IMessageSession messageSession)
            {
                this.messageSession = messageSession;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                return messageSession.Publish<MyEvent>();
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            readonly IMessageSession messageSession;

            class MyEvent : IEvent
            {
            }
        }
    }
}
