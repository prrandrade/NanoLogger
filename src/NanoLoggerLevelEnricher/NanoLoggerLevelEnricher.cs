namespace NanoLoggerLevelEnricher
{
    using System;
    using Serilog.Core;
    using Serilog.Events;

    public class NanoLoggerLevelEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var crudeLogName = logEvent.Level switch
            {
                LogEventLevel.Verbose => "TRC", //LogLevel.Trace
                LogEventLevel.Debug => "DBG", //LogLevel.Debug
                LogEventLevel.Information => "INF", //LogLevel.Information
                LogEventLevel.Warning => "WRN", //LogLevel.Warning
                LogEventLevel.Error => "ERR", //LogLevel.Error
                LogEventLevel.Fatal => "CRT", //LogLevel.Critical
                _ => throw new ArgumentOutOfRangeException()
            };

            var coloredLogName = logEvent.Level switch
            {
                LogEventLevel.Verbose => "\x1b[38;5;0007m" + "TRC", //LogLevel.Trace
                LogEventLevel.Debug => "\x1b[38;5;0007m" + "DBG", //LogLevel.Debug
                LogEventLevel.Information => "\x1b[38;2;0;150;0m" + "INF", //LogLevel.Information
                LogEventLevel.Warning => "\x1b[38;2;200;200;0m" + "WRN", //LogLevel.Warning
                LogEventLevel.Error => "\x1b[38;2;255;0;0m" + "ERR", //LogLevel.Error
                LogEventLevel.Fatal => "\x1b[38;2;255;255;255m\x1b[48;2;255;0;0m" + "CRT", //LogLevel.Critical
                _ => throw new ArgumentOutOfRangeException()
            };
            
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("LogLevel", crudeLogName));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ColoredLogLevel", coloredLogName));
        }
    }
}
