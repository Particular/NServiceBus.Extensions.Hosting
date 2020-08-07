namespace AcceptanceTests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NServiceBus;
    using NUnit.Framework;

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

            try
            {
                await host.StartAsync();

                Assert.NotNull(host.Services.GetService<IMessageSession>());
            }
            finally
            {
                await host.StopAsync();
            }
        }
    }
}