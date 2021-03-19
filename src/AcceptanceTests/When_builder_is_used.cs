namespace AcceptanceTests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NServiceBus;
    using NServiceBus.Extensions.Hosting;
    using NUnit.Framework;

    [TestFixture]
    public class When_builder_is_used
    {
        [Test]
        public async Task Should_work()
        {
            var customBuilder = new CustomEndpointConfigurationBuilder();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<IEndpointConfigurationBuilder>(_ => customBuilder);
                })
                .UseNServiceBus()
                .Build();

            await host.StartAsync();
            await host.StopAsync();

            Assert.That(customBuilder.BuildWasCalled, Is.True);
        }

        class CustomEndpointConfigurationBuilder : IEndpointConfigurationBuilder
        {
            public async Task<EndpointConfiguration> Build()
            {
                await Task.Yield();

                BuildWasCalled = true;

                var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
                endpointConfiguration.SendOnly();
                endpointConfiguration.UseTransport<LearningTransport>();
                return endpointConfiguration;
            }

            public bool BuildWasCalled { get; private set; }
        }
    }
}