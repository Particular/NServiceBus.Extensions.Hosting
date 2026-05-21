#nullable enable
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
    public class When_logging_with_host_builder
    {
        [Test]
        public async Task Should_integrate_with_default_host_logging()
        {
            var collectingLoggerProvider = new CollectingLoggerProvider();

            var expectedLogMessage = "We want to see this";
            var notExpectedLogMessage = "We don't want to see this";

#pragma warning disable CS0618 // Type or member is obsolete
            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Warning);
                    logging.AddProvider(collectingLoggerProvider);
                })
                .UseNServiceBus(hostBuilderContext =>
                {
                    var logger = LogManager.GetLogger("TestLogger");
                    logger.Warn(expectedLogMessage);
                    logger.Debug(notExpectedLogMessage);

                    var endpointConfiguration = new EndpointConfiguration("NSBRepro");
                    endpointConfiguration.UseTransport(new LearningTransport { StorageDirectory = TestContext.CurrentContext.TestDirectory });
                    endpointConfiguration.UseSerialization<SystemJsonSerializer>();

                    return endpointConfiguration;
                })
#pragma warning restore CS0618 // Type or member is obsolete
                .Build();

            try
            {
                await host.StartAsync();
                Assert.That(collectingLoggerProvider.LogEntries, Is.SupersetOf([("TestLogger", LogLevel.Warning, expectedLogMessage)]));
                Assert.That(collectingLoggerProvider.LogEntries, Is.Not.SupersetOf(new List<(string, LogLevel, string)> { ("TestLogger", LogLevel.Debug, notExpectedLogMessage) }));
            }
            finally
            {
                await host.StopAsync();
            }
        }
    }
}
