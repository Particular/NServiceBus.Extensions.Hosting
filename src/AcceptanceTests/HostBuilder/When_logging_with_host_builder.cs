﻿namespace AcceptanceTests
{
    using System;
    using System.Text;
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
            var builder = new StringBuilder();

            var expectedLogMessage = "We want to see this";
            var notExpectedLogMessage = "We don't want to see this";

            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Warning);
                    logging.AddProvider(new StringBuilderProvider(builder));
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
                .Build();

            try
            {
                await host.StartAsync();

                var actual = builder.ToString();
                Assert.That(actual, Does.Contain(expectedLogMessage));
                Assert.That(actual, Does.Not.Contain(notExpectedLogMessage));
            }
            finally
            {
                await host.StopAsync();
            }
        }

        class StringBuilderProvider : ILoggerProvider, ILogger
        {
            public StringBuilderProvider(StringBuilder builder)
            {
                this.builder = builder;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var message = formatter(state, exception);
                builder.AppendLine($"[{logLevel}] {message}");
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
            }

            public ILogger CreateLogger(string categoryName)
            {
                return this;
            }

            readonly StringBuilder builder;
        }
    }
}
