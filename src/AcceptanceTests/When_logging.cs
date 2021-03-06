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
    public class When_logging
    {
        [Test]
        public async Task Should_integrate_with_default_host_logging()
        {
            var builder = new StringBuilder();

            var ExpectedLogMessage = "We want to see this";
            var NotExpectedLogMessage = "We don't want to see this";

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
                    logger.Warn(ExpectedLogMessage);
                    logger.Debug(NotExpectedLogMessage);
                    var endpointConfiguration = new EndpointConfiguration("NSBRepro");
                    var transport = endpointConfiguration.UseTransport<LearningTransport>();
                    transport.StorageDirectory(TestContext.CurrentContext.TestDirectory);
                    return endpointConfiguration;
                })
                .Build();

            try
            {
                await host.StartAsync();

                StringAssert.Contains(ExpectedLogMessage, builder.ToString());
                StringAssert.DoesNotContain(NotExpectedLogMessage, builder.ToString());
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