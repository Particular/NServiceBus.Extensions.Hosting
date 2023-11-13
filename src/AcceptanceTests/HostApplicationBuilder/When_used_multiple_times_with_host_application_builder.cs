namespace AcceptanceTests
{
    using System;
    using Microsoft.Extensions.Hosting;
    using NServiceBus;
    using NUnit.Framework;

    [TestFixture]
    public class When_used_multiple_times_with_host_application_builder
    {
        [Test]
        public void Throws_on_same_host()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var hostBuilder = Host.CreateApplicationBuilder();

                hostBuilder.UseNServiceBus(() =>
                {
                    var endpointConfiguration = new EndpointConfiguration("NSBRepro");
                    endpointConfiguration.SendOnly();
                    endpointConfiguration.UseTransport(new LearningTransport());
                    return endpointConfiguration;
                });

                hostBuilder.UseNServiceBus(() =>
                {
                    var endpointConfiguration = new EndpointConfiguration("NSBRepro1");
                    endpointConfiguration.SendOnly();
                    endpointConfiguration.UseTransport(new LearningTransport());
                    return endpointConfiguration;
                });

                hostBuilder.Build();
            });
        }

        [Test]
        public void Does_not_throw_on_different_hosts()
        {
            Assert.DoesNotThrow(() =>
            {
                var hostBuilder1 = Host.CreateApplicationBuilder();
                hostBuilder1.UseNServiceBus(() =>
                {
                    var endpointConfiguration = new EndpointConfiguration("NSBRepro1");
                    endpointConfiguration.SendOnly();
                    endpointConfiguration.UseTransport(new LearningTransport());
                    return endpointConfiguration;
                });

                hostBuilder1.Build();

                var hostBuilder2 = Host.CreateApplicationBuilder();
                hostBuilder2.UseNServiceBus(() =>
                {
                    var endpointConfiguration = new EndpointConfiguration("NSBRepro1");
                    endpointConfiguration.SendOnly();
                    endpointConfiguration.UseTransport(new LearningTransport());
                    return endpointConfiguration;
                });

                hostBuilder2.Build();
            });
        }
    }
}