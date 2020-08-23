namespace NanoLoggerLevelEnricher
{
    using Serilog.Core;
    using Serilog.Events;

    public class NanoLoggerLevelEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            string crudeLogName;
            string coloredLogName;
            switch (logEvent.Level)
            {
                case LogEventLevel.Verbose:
                    crudeLogName = "TRC"; // LogLevel.Trace
                    coloredLogName = "\x1b[38;5;0007m" + "TRC";
                    break;
                case LogEventLevel.Debug:
                    crudeLogName = "DBG"; // LogLevel.Debug
                    coloredLogName = "\x1b[38;5;0007m" + "DBG";
                    break;
                case LogEventLevel.Information:
                    crudeLogName = "INF"; // LogLevel.Information
                    coloredLogName = "\x1b[38;2;0;150;0m" + "INF";
                    break;
                case LogEventLevel.Warning:
                    crudeLogName = "WRN"; // LogLevel.Warning
                    coloredLogName = "\x1b[38;2;200;200;0m" + "WRN";
                    break;
                case LogEventLevel.Error:
                    crudeLogName = "ERR"; // LogLevel.Error
                    coloredLogName = "\x1b[38;2;255;0;0m" + "ERR";
                    break;
                default:
                    crudeLogName = "CRT"; // LogLevel.Critical
                    coloredLogName = "\x1b[38;2;255;255;255m\x1b[48;2;255;0;0m" + "CRT";
                    break;
            }

            var logLevelProperty = propertyFactory.CreateProperty("LogLevel", crudeLogName);
            logEvent.AddOrUpdateProperty(logLevelProperty);

            var coloredLogLevelProperty = propertyFactory.CreateProperty("ColoredLogLevel", coloredLogName); 
            logEvent.AddOrUpdateProperty(coloredLogLevelProperty);
        }
    }
}
