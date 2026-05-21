namespace AcceptanceTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NServiceBus;
    using NServiceBus.Logging;
    using NUnit.Framework;
    using LogLevel = Microsoft.Extensions.Logging.LogLevel;

    [TestFixture]
    public class When_logging_with_host_application_builder
    {
        [Test]
        public async Task Should_integrate_with_default_host_logging()
        {
            var collectingLoggerProvider = new CollectingLoggerProvider();

            var expectedLogMessage = "We want to see this";
            var notExpectedLogMessage = "We don't want to see this";

            var hostBuilder = Host.CreateApplicationBuilder();
            hostBuilder.Logging.ClearProviders();
            hostBuilder.Logging.SetMinimumLevel(LogLevel.Warning);
            hostBuilder.Logging.AddProvider(collectingLoggerProvider);

            var endpointConfiguration = new EndpointConfiguration("NSBRepro");
            endpointConfiguration.UseTransport(new LearningTransport { StorageDirectory = TestContext.CurrentContext.TestDirectory });
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

#pragma warning disable CS0618 // Type or member is obsolete
            hostBuilder.UseNServiceBus(endpointConfiguration);
#pragma warning restore CS0618 // Type or member is obsolete

            var logger = LogManager.GetLogger("TestLogger");
            logger.Warn(expectedLogMessage);
            logger.Debug(notExpectedLogMessage);

            var host = hostBuilder.Build();

            try
            {
                await host.StartAsync();

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(collectingLoggerProvider.LogEntries, Is.SupersetOf([("TestLogger", LogLevel.Warning, expectedLogMessage)]));
                    Assert.That(collectingLoggerProvider.LogEntries, Is.Not.SupersetOf(new List<(string, LogLevel, string)> { ("TestLogger", LogLevel.Debug, notExpectedLogMessage) }));
                }
            }
            finally
            {
                await host.StopAsync();
            }
        }
    }
}