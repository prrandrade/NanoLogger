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
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return LogEventLevel.Verbose;
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                case LogLevel.Information:
                    return LogEventLevel.Information;
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Critical:
                    return LogEventLevel.Fatal;
                case LogLevel.None:
                    return (LogEventLevel) 1 + (int) LogEventLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}
