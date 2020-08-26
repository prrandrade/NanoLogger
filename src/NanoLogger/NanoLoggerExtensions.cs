namespace NanoLogger
{
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NanoLoggerLevelEnricher;
    using PropertyRetriever;
    using PropertyRetriever.Interfaces;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Exceptions;
    using Services;

    public static class NanoLoggerExtensions
    {
        public static IServiceCollection AddNanoLogger(this IServiceCollection @this)
        {
            @this.AddPropertyRetriever();

            var seqControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var fileControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var consoleControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);

            var manager = new NanoLoggerManager(seqControlLevelSwitch, fileControlLevelSwitch, consoleControlLevelSwitch);
            @this.AddSingleton<INanoLoggerManager>(manager);
            LoggerConfiguration logger;

            using (var scope = @this.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                logger = GetLoggerConfiguration(manager, scope.ServiceProvider.GetService<IPropertyRetriever>());
            }

            @this.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(logger.CreateLogger());
            });

            return @this;
        }

        public static LoggerConfiguration GetLoggerConfiguration(NanoLoggerManager manager, IPropertyRetriever propertyRetriever)
        {
            #region Options for seq log

            string seqAddress = null;
            string seqApiKey = null;

            #endregion

            #region Options for file log

            var fileRollingInterval = RollingInterval.Hour;
            var fileMessageTemplate = "";

            #endregion

            #region Options for console log

            var consoleMessageTemplate = "";

            #endregion

            var serviceName = propertyRetriever.RetrieveServiceName();
            var withSeqLog = propertyRetriever.CheckFromCommandLine("withSeqLog");
            var withConsoleLog = propertyRetriever.CheckFromCommandLine("withConsoleLog");
            var withFileLog = propertyRetriever.CheckFromCommandLine("withFileLog");

            #region Retrieving options for seq log

            if (withSeqLog)
            {
                seqAddress = propertyRetriever.RetrieveFromCommandLineOrEnvironment("seqLogAddress", "seqLogAddress", null);
                seqApiKey = propertyRetriever.RetrieveFromCommandLineOrEnvironment("seqApiKey", "seqApiKey", null);
                manager.SetSeqLoggingLevel(propertyRetriever.RetrieveFromCommandLineOrEnvironment("seqMinimumLogEventLevel", "seqMinimumLogEventLevel", LogLevel.None));
            }

            #endregion

            #region Retrieving options for file log

            if (withFileLog)
            {
                fileMessageTemplate = propertyRetriever.RetrieveFromCommandLineOrEnvironment("fileMessageTemplate", "fileMessageTemplate", "{Timestamp} [{LogLevel}] (MachineName: {MachineName}) (Thread: {ThreadId}) {Message} {Exception}{NewLine}");
                fileRollingInterval = propertyRetriever.RetrieveFromCommandLineOrEnvironment("fileRollingInterval", "fileRollingInterval", RollingInterval.Hour);
                manager.SetFileLoggingLevel(propertyRetriever.RetrieveFromCommandLineOrEnvironment("fileMinimumLogEventLevel", "fileMinimumLogEventLevel", LogLevel.None));
            }

            #endregion

            #region Retrieving options for console log

            if (withConsoleLog)
            {
                consoleMessageTemplate = propertyRetriever.RetrieveFromCommandLineOrEnvironment("consoleMessageTemplate", "consoleMessageTemplate", "{Timestamp} [{LogLevel}] (MachineName: {MachineName}) (Thread: {ThreadId}) {Message} {Exception}{NewLine}").Replace("{LogLevel}", "{ColoredLogLevel}");
                manager.SetConsoleLoggingLevel(propertyRetriever.RetrieveFromCommandLineOrEnvironment("consoleMinimumLogEventLevel", "consoleMinimumLogEventLevel", LogLevel.None));
            }

            #endregion

            var logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithLoggerLevel()
                .MinimumLevel.Verbose();

            if (withSeqLog) 
            {
                logger = logger
                    .WriteTo.Seq(
                        serverUrl: seqAddress, 
                        apiKey: seqApiKey, 
                        controlLevelSwitch: manager.SeqLoggingLevelSwitch);
            }

            if (withFileLog)
            {
                logger = logger
                    .WriteTo.File(
                        path: $"log\\log_{serviceName}_.txt",
                        levelSwitch: manager.FileLoggingLevelSwitch,
                        rollingInterval: fileRollingInterval,
                        outputTemplate: fileMessageTemplate,
                        shared: true);
            }

            if (withConsoleLog)
            {
                logger = logger
                    .WriteTo.Console(
                        outputTemplate: consoleMessageTemplate,
                        levelSwitch: manager.ConsoleLoggingLevelSwitch);
            }

            return logger;
        }
    }
}
