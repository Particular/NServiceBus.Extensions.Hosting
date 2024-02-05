namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Collections.Concurrent;
    using Logging;

    class DeferredLoggerFactory : ILoggerFactory
    {
        readonly ConcurrentBag<DeferredLogger> acquiredLoggers = [];

        public ILog GetLogger(Type type) => GetLogger(type.FullName);

        public ILog GetLogger(string name)
        {
            var deferredLogger = new DeferredLogger(name);
            acquiredLoggers.Add(deferredLogger);
            return deferredLogger;
        }

        public void FlushAll(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            while (acquiredLoggers.TryTake(out var logger))
            {
                logger.Flush(loggerFactory);
            }
        }
    }
}