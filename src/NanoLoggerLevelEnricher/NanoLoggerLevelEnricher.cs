namespace NanoLoggerLevelEnricher
{
    using System;
    using System.ComponentModel;
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
                _ => "CRT", //LogLevel.Critical
            };

            var coloredLogName = logEvent.Level switch
            {
                LogEventLevel.Verbose => "\x1b[38;5;0007m" + "TRC",
                LogEventLevel.Debug => "\x1b[38;5;0007m" + "DBG",
                LogEventLevel.Information => "\x1b[38;2;0;150;0m" + "INF",
                LogEventLevel.Warning => "\x1b[38;2;200;200;0m" + "WRN",
                LogEventLevel.Error => "\x1b[38;2;255;0;0m" + "ERR",
                _ => "\x1b[38;2;255;255;255m\x1b[48;2;255;0;0m" + "CRT"
            };

            var logLevelProperty = propertyFactory.CreateProperty("LogLevel", crudeLogName);
            logEvent.AddOrUpdateProperty(logLevelProperty);

            var coloredLogLevelProperty = propertyFactory.CreateProperty("ColoredLogLevel", coloredLogName); 
            logEvent.AddOrUpdateProperty(coloredLogLevelProperty);
        }
    }
}
