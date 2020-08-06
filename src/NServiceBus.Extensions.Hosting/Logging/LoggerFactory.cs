namespace NServiceBus.Extensions.Hosting
{
    using System;
    using Logging;
    using IMsLoggingFactory = Microsoft.Extensions.Logging.ILoggerFactory;

    class LoggerFactory : ILoggerFactory
    {
        public LoggerFactory(IMsLoggingFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public ILog GetLogger(Type type) => new Logger(loggerFactory.CreateLogger(type.FullName));

        public ILog GetLogger(string name) => new Logger(loggerFactory.CreateLogger(name));

        IMsLoggingFactory loggerFactory;
    }
}