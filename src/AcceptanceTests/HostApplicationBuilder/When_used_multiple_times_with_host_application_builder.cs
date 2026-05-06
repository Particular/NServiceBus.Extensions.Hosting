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

                var endpointConfiguration1 = new EndpointConfiguration("NSBRepro");
                endpointConfiguration1.SendOnly();
                endpointConfiguration1.UseTransport(new LearningTransport());
                endpointConfiguration1.UseSerialization<SystemJsonSerializer>();

#pragma warning disable CS0618 // Type or member is obsolete
                hostBuilder.UseNServiceBus(endpointConfiguration1);
#pragma warning restore CS0618 // Type or member is obsolete

                var endpointConfiguration2 = new EndpointConfiguration("NSBRepro1");
                endpointConfiguration2.SendOnly();
                endpointConfiguration2.UseTransport(new LearningTransport());
                endpointConfiguration2.UseSerialization<SystemJsonSerializer>();

#pragma warning disable CS0618 // Type or member is obsolete
                hostBuilder.UseNServiceBus(endpointConfiguration2);
#pragma warning restore CS0618 // Type or member is obsolete

                hostBuilder.Build();
            });
        }

        [Test]
        public void Does_not_throw_on_different_hosts()
        {
            Assert.DoesNotThrow(() =>
            {
                var hostBuilder1 = Host.CreateApplicationBuilder();

                var endpointConfiguration1 = new EndpointConfiguration("NSBRepro1");
                endpointConfiguration1.SendOnly();
                endpointConfiguration1.UseTransport(new LearningTransport());
                endpointConfiguration1.UseSerialization<SystemJsonSerializer>();

#pragma warning disable CS0618 // Type or member is obsolete
                hostBuilder1.UseNServiceBus(endpointConfiguration1);
#pragma warning restore CS0618 // Type or member is obsolete

                hostBuilder1.Build();

                var hostBuilder2 = Host.CreateApplicationBuilder();

                var endpointConfiguration2 = new EndpointConfiguration("NSBRepro1");
                endpointConfiguration2.SendOnly();
                endpointConfiguration2.UseTransport(new LearningTransport());
                endpointConfiguration2.UseSerialization<SystemJsonSerializer>();

#pragma warning disable CS0618 // Type or member is obsolete
                hostBuilder2.UseNServiceBus(endpointConfiguration2);
#pragma warning restore CS0618 // Type or member is obsolete

                hostBuilder2.Build();
            });
        }
    }
}