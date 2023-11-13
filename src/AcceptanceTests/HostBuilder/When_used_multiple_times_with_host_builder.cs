namespace AcceptanceTests
{
    using System;
    using Microsoft.Extensions.Hosting;
    using NServiceBus;
    using NUnit.Framework;

    [TestFixture]
    public class When_used_multiple_times_with_host_builder
    {
        [Test]
        public void Throws_on_same_host()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Host.CreateDefaultBuilder()
                    .UseNServiceBus(hostBuilderContext =>
                    {
                        var endpointConfiguration = new EndpointConfiguration("NSBRepro");
                        endpointConfiguration.SendOnly();
                        endpointConfiguration.UseTransport(new LearningTransport());
                        return endpointConfiguration;
                    })
                    .UseNServiceBus(hostBuilderContext =>
                    {
                        var endpointConfiguration = new EndpointConfiguration("NSBRepro1");
                        endpointConfiguration.SendOnly();
                        endpointConfiguration.UseTransport(new LearningTransport());
                        return endpointConfiguration;
                    })
                    .Build();
            });
        }

        [Test]
        public void Does_not_throw_on_different_hosts()
        {
            Assert.DoesNotThrow(() =>
            {
                Host.CreateDefaultBuilder()
                    .UseNServiceBus(hostBuilderContext =>
                    {
                        var endpointConfiguration = new EndpointConfiguration("NSBRepro1");
                        endpointConfiguration.SendOnly();
                        endpointConfiguration.UseTransport(new LearningTransport());
                        return endpointConfiguration;
                    })
                    .Build();

                Host.CreateDefaultBuilder()
                    .UseNServiceBus(hostBuilderContext =>
                    {
                        var endpointConfiguration = new EndpointConfiguration("NSBRepro1");
                        endpointConfiguration.SendOnly();
                        endpointConfiguration.UseTransport(new LearningTransport());
                        return endpointConfiguration;
                    })
                    .Build();
            });
        }
    }
}