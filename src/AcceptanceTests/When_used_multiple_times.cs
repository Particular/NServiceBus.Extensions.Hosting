namespace AcceptanceTests
{
    using System;
    using Microsoft.Extensions.Hosting;
    using NServiceBus;
    using NUnit.Framework;

    [TestFixture]
    public class When_used_multiple_times
    {
        [Test]
        public void Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Host.CreateDefaultBuilder()
                    .UseNServiceBus(hostBuilderContext =>
                    {
                        var endpointConfiguration = new EndpointConfiguration("NSBRepro");
                        endpointConfiguration.SendOnly();
                        endpointConfiguration.UseTransport<LearningTransport>();
                        return endpointConfiguration;
                    })
                    .UseNServiceBus(hostBuilderContext =>
                    {
                        var endpointConfiguration = new EndpointConfiguration("NSBRepro");
                        endpointConfiguration.SendOnly();
                        endpointConfiguration.UseTransport<LearningTransport>();
                        return endpointConfiguration;
                    })
                    .Build();
            });
        }
    }
}