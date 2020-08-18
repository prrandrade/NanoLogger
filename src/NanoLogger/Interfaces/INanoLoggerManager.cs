namespace NanoLogger.Interfaces
{
    using Microsoft.Extensions.Logging;

    public interface INanoLoggerManager
    {
        public void SetSeqLoggingLevel(LogLevel logLevel);

        public void SetFileLoggingLevel(LogLevel logLevel);

        public void SetConsoleLoggingLevel(LogLevel logLevel);
    }
}
