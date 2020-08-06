﻿namespace AcceptanceTests
{
    using Microsoft.Extensions.Hosting;
    using NUnit.Framework;
    using NServiceBus;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    [TestFixture]
    public class When_session_is_accessed_in_hosted_service_ctor
    {
        [Test]
        public void Should_throw()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var host = Host.CreateDefaultBuilder()
                .UseNServiceBus(hostBuilderContext =>
                {
                  var endpointConfiguration = new EndpointConfiguration("NSBRepro");
                  endpointConfiguration.SendOnly();
                  endpointConfiguration.UseTransport<LearningTransport>();
                  return endpointConfiguration;
                })
                .ConfigureServices((ctx, serviceProvider) => serviceProvider.AddHostedService<HostedServiceThatAccessSessionInCtor>()).Build();

                await host.StartAsync();
            });

            StringAssert.Contains("The message session can't be used before the NServiceBus hosted service is started", ex.Message);
        }

        class HostedServiceThatAccessSessionInCtor : IHostedService
        {
            public HostedServiceThatAccessSessionInCtor(IServiceProvider serviceProvider)
            {
                serviceProvider.GetService<IMessageSession>().Publish<MyEvent>().GetAwaiter().GetResult();
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