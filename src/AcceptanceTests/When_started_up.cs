namespace AcceptanceTests
{
    using Microsoft.Extensions.Hosting;
    using NUnit.Framework;
    using NServiceBus;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;

    [TestFixture]
    public class When_started_up
    {
        [Test]
        public async Task Message_session_should_be_available_in_DI()
        {
            var host = Host.CreateDefaultBuilder()
                .UseNServiceBus(hostBuilderContext =>
                {
                    var endpointConfiguration = new EndpointConfiguration("NSBRepro");
                    endpointConfiguration.SendOnly();
                    endpointConfiguration.UseTransport<LearningTransport>();
                    return endpointConfiguration;
                })
                .Build();

            await host.StartAsync();

            Assert.NotNull(host.Services.GetService<IMessageSession>());

            await host.StopAsync();
        }
    }
}
