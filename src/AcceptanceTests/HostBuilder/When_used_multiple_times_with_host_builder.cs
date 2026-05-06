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
#pragma warning disable CS0618 // Type or member is obsolete
                Host.CreateDefaultBuilder()
                    .UseNServiceBus(hostBuilderContext =>
                    {
                        var endpointConfiguration = new EndpointConfiguration("NSBRepro");
                        endpointConfiguration.SendOnly();
                        endpointConfiguration.UseTransport(new LearningTransport());
                        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
                        return endpointConfiguration;
                    })
                    .UseNServiceBus(hostBuilderContext =>
                    {
                        var endpointConfiguration = new EndpointConfiguration("NSBRepro1");
                        endpointConfiguration.SendOnly();
                        endpointConfiguration.UseTransport(new LearningTransport());
                        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
                        return endpointConfiguration;
                    })
#pragma warning restore CS0618 // Type or member is obsolete
                    .Build();
            });
        }

        [Test]
        public void Does_not_throw_on_different_hosts()
        {
            Assert.DoesNotThrow(() =>
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Host.CreateDefaultBuilder()
                    .UseNServiceBus(hostBuilderContext =>
                    {
                        var endpointConfiguration = new EndpointConfiguration("NSBRepro1");
                        endpointConfiguration.SendOnly();
                        endpointConfiguration.UseTransport(new LearningTransport());
                        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
                        return endpointConfiguration;
                    })
#pragma warning restore CS0618 // Type or member is obsolete
                    .Build();

#pragma warning disable CS0618 // Type or member is obsolete
                Host.CreateDefaultBuilder()
                    .UseNServiceBus(hostBuilderContext =>
                    {
                        var endpointConfiguration = new EndpointConfiguration("NSBRepro1");
                        endpointConfiguration.SendOnly();
                        endpointConfiguration.UseTransport(new LearningTransport());
                        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
                        return endpointConfiguration;
                    })
#pragma warning restore CS0618 // Type or member is obsolete
                    .Build();
            });
        }
    }
}