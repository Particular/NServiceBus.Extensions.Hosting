namespace AcceptanceTests
{
    using System;
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
        public void Should_throw()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
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
            });

            StringAssert.Contains("The message session can't be used before NServiceBus is started", ex.Message);
        }

        class HostedServiceThatAccessSessionInCtor : IHostedService
        {
            public HostedServiceThatAccessSessionInCtor(IMessageSession messageSession)
            {
                messageSession.Publish<MyEvent>().GetAwaiter().GetResult();
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            class MyEvent : IEvent
            {
            }
        }
    }
}
