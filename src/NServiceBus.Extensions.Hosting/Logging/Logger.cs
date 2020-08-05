namespace NServiceBus.Extensions.Hosting
{
    using System;
    using Logging;
    using Microsoft.Extensions.Logging;
    using LogLevel = Microsoft.Extensions.Logging.LogLevel;

    class Logger : ILog
    {
        public Logger(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsDebugEnabled => logger.IsEnabled(LogLevel.Debug);

        public bool IsInfoEnabled => logger.IsEnabled(LogLevel.Information);

        public bool IsWarnEnabled => logger.IsEnabled(LogLevel.Warning);

        public bool IsErrorEnabled => logger.IsEnabled(LogLevel.Error);

        public bool IsFatalEnabled => logger.IsEnabled(LogLevel.Critical);

        public void Debug(string message)
        {
            logger.LogDebug(message);
        }

        public void Debug(string message, Exception exception)
        {
            logger.LogDebug(exception, message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            logger.LogDebug(format, args);
        }

        public void Info(string message)
        {
            logger.LogInformation(message);
        }

        public void Info(string message, Exception exception)
        {
            logger.LogInformation(exception, message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            logger.LogInformation(format, args);
        }

        public void Warn(string message)
        {
            logger.LogWarning(message);
        }

        public void Warn(string message, Exception exception)
        {
            logger.LogWarning(exception, message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            logger.LogWarning(format, args);
        }

        public void Error(string message)
        {
            logger.LogError(message);
        }

        public void Error(string message, Exception exception)
        {
            logger.LogError(exception, message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            logger.LogError(format, args);
        }

        public void Fatal(string message)
        {
            logger.LogCritical(message);
        }

        public void Fatal(string message, Exception exception)
        {
            logger.LogCritical(exception, message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            logger.LogCritical(format, args);
        }

        ILogger logger;
    }
}