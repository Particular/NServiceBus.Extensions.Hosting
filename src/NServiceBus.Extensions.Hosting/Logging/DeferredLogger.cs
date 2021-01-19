namespace NServiceBus.Extensions.Hosting
{
    using System;
    using System.Collections.Concurrent;
    using Logging;
    using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

    class DeferredLogger :
        ILog
    {
        public DeferredLogger(string name)
        {
            this.name = name;
        }

        // capturing everything just in case when the logger is not yet set
        public bool IsDebugEnabled => logger == null || logger.IsDebugEnabled;
        public bool IsInfoEnabled => logger == null || logger.IsInfoEnabled;
        public bool IsWarnEnabled => logger == null || logger.IsWarnEnabled;
        public bool IsErrorEnabled => logger == null || logger.IsErrorEnabled;
        public bool IsFatalEnabled => logger == null || logger.IsFatalEnabled;

        public void Debug(string message)
        {
            if (logger != null)
            {
                logger.Debug(message);
                return;
            }

            deferredMessageLogs.Enqueue((LogLevel.Debug, message));
        }

        public void Debug(string message, Exception exception)
        {
            if (logger != null)
            {
                logger.Debug(message, exception);
                return;
            }

            deferredExceptionLogs.Enqueue((LogLevel.Debug, message, exception));
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (logger != null)
            {
                logger.DebugFormat(format, args);
                return;
            }

            deferredFormatLogs.Enqueue((LogLevel.Debug, format, args));
        }

        public void Info(string message)
        {
            if (logger != null)
            {
                logger.Info(message);
                return;
            }

            deferredMessageLogs.Enqueue((LogLevel.Info, message));
        }

        public void Info(string message, Exception exception)
        {
            if (logger != null)
            {
                logger.Info(message, exception);
                return;
            }

            deferredExceptionLogs.Enqueue((LogLevel.Info, message, exception));
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (logger != null)
            {
                logger.InfoFormat(format, args);
                return;
            }

            deferredFormatLogs.Enqueue((LogLevel.Info, format, args));
        }

        public void Warn(string message)
        {
            if (logger != null)
            {
                logger.Warn(message);
                return;
            }

            deferredMessageLogs.Enqueue((LogLevel.Warn, message));
        }

        public void Warn(string message, Exception exception)
        {
            if (logger != null)
            {
                logger.Warn(message, exception);
                return;
            }

            deferredExceptionLogs.Enqueue((LogLevel.Warn, message, exception));
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (logger != null)
            {
                logger.WarnFormat(format, args);
                return;
            }

            deferredFormatLogs.Enqueue((LogLevel.Warn, format, args));
        }

        public void Error(string message)
        {
            if (logger != null)
            {
                logger.Error(message);
                return;
            }

            deferredMessageLogs.Enqueue((LogLevel.Error, message));
        }

        public void Error(string message, Exception exception)
        {
            if (logger != null)
            {
                logger.Error(message, exception);
                return;
            }

            deferredExceptionLogs.Enqueue((LogLevel.Error, message, exception));
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (logger != null)
            {
                logger.ErrorFormat(format, args);
                return;
            }

            deferredFormatLogs.Enqueue((LogLevel.Error, format, args));
        }

        public void Fatal(string message)
        {
            if (logger != null)
            {
                logger.Fatal(message);
                return;
            }

            deferredMessageLogs.Enqueue((LogLevel.Fatal, message));
        }

        public void Fatal(string message, Exception exception)
        {
            if (logger != null)
            {
                logger.Fatal(message, exception);
                return;
            }

            deferredExceptionLogs.Enqueue((LogLevel.Fatal, message, exception));
        }

        public void FatalFormat(string format, params object[] args)
        {
            if (logger != null)
            {
                logger.FatalFormat(format, args);
                return;
            }

            deferredFormatLogs.Enqueue((LogLevel.Fatal, format, args));
        }

        public void Flush(ILoggerFactory loggerFactory)
        {
            logger = new Logger(loggerFactory.CreateLogger(name));

            while (deferredMessageLogs.TryDequeue(out var entry))
            {
                switch (entry.level)
                {
                    case LogLevel.Debug:
                        logger.Debug(entry.message);
                        break;
                    case LogLevel.Info:
                        logger.Info(entry.message);
                        break;
                    case LogLevel.Warn:
                        logger.Warn(entry.message);
                        break;
                    case LogLevel.Error:
                        logger.Error(entry.message);
                        break;
                    case LogLevel.Fatal:
                        logger.Fatal(entry.message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            while (deferredExceptionLogs.TryDequeue(out var entry))
            {
                switch (entry.level)
                {
                    case LogLevel.Debug:
                        logger.Debug(entry.message, entry.exception);
                        break;
                    case LogLevel.Info:
                        logger.Info(entry.message, entry.exception);
                        break;
                    case LogLevel.Warn:
                        logger.Warn(entry.message, entry.exception);
                        break;
                    case LogLevel.Error:
                        logger.Error(entry.message, entry.exception);
                        break;
                    case LogLevel.Fatal:
                        logger.Fatal(entry.message, entry.exception);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            while (deferredFormatLogs.TryDequeue(out var entry))
            {
                switch (entry.level)
                {
                    case LogLevel.Debug:
                        logger.DebugFormat(entry.format, entry.args);
                        break;
                    case LogLevel.Info:
                        logger.InfoFormat(entry.format, entry.args);
                        break;
                    case LogLevel.Warn:
                        logger.WarnFormat(entry.format, entry.args);
                        break;
                    case LogLevel.Error:
                        logger.ErrorFormat(entry.format, entry.args);
                        break;
                    case LogLevel.Fatal:
                        logger.FatalFormat(entry.format, entry.args);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        readonly ConcurrentQueue<(LogLevel level, string message)> deferredMessageLogs = new ConcurrentQueue<(LogLevel level, string message)>();
        readonly ConcurrentQueue<(LogLevel level, string message, Exception exception)> deferredExceptionLogs = new ConcurrentQueue<(LogLevel level, string message, Exception exception)>();
        readonly ConcurrentQueue<(LogLevel level, string format, object[] args)> deferredFormatLogs = new ConcurrentQueue<(LogLevel level, string format, object[] args)>();

        string name;

        ILog logger;
    }
}