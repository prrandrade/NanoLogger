namespace NanoLogger.Services
{
    using System;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Serilog.Core;
    using Serilog.Events;

    public class NanoLoggerManager : INanoLoggerManager
    {
        public LoggingLevelSwitch SeqLoggingLevelSwitch { get; }
        public LoggingLevelSwitch FileLoggingLevelSwitch { get; }
        public LoggingLevelSwitch ConsoleLoggingLevelSwitch { get; }

        public NanoLoggerManager(LoggingLevelSwitch seqLoggingLevelSwitch, LoggingLevelSwitch fileLoggingLevelSwitch, LoggingLevelSwitch consoleLoggingLevelSwitch)
        {
            SeqLoggingLevelSwitch = seqLoggingLevelSwitch;
            FileLoggingLevelSwitch = fileLoggingLevelSwitch;
            ConsoleLoggingLevelSwitch = consoleLoggingLevelSwitch;
        }

        public void SetSeqLoggingLevel(LogLevel logLevel)
        {
            SeqLoggingLevelSwitch.MinimumLevel = ConvertLogLevel(logLevel);
        }

        public void SetFileLoggingLevel(LogLevel logLevel)
        {
            FileLoggingLevelSwitch.MinimumLevel = ConvertLogLevel(logLevel);
        }

        public void SetConsoleLoggingLevel(LogLevel logLevel)
        {
            ConsoleLoggingLevelSwitch.MinimumLevel = ConvertLogLevel(logLevel);
        }

        private static LogEventLevel ConvertLogLevel(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => LogEventLevel.Verbose,
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Information => LogEventLevel.Information,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Critical => LogEventLevel.Fatal,
                LogLevel.None => (LogEventLevel) 1 + (int) LogEventLevel.Fatal,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
            };
        }
    }
}
