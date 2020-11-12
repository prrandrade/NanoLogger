namespace NanoLogger
{
    using Enrichers;
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using PropertyRetriever;
    using PropertyRetriever.Interfaces;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Exceptions;
    using Services;

    public static class NanoLoggerExtensions
    {
        public static IServiceCollection AddNanoLogger(this IServiceCollection @this, bool withDefaultConsoleLog = false)
        {
            @this.AddPropertyRetriever();

            var seqControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Warning);
            var fileControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var consoleControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);

            var manager = new NanoLoggerManager(seqControlLevelSwitch, fileControlLevelSwitch, consoleControlLevelSwitch);
            @this.AddSingleton<INanoLoggerManager>(manager);
            LoggerConfiguration logger;

            using (var scope = @this.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                logger = GetLoggerConfiguration(manager, scope.ServiceProvider.GetService<IPropertyRetriever>(), withDefaultConsoleLog);
            }

            @this.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(logger.CreateLogger());
            });

            return @this;
        }

        public static LoggerConfiguration WithSeqLog(this LoggerConfiguration logger, NanoLoggerManager manager, IPropertyRetriever propertyRetriever)
        {
            var withSeqLog = propertyRetriever.CheckFromCommandLine("withSeqLog");

            if (withSeqLog)
            {
                var seqAddress = propertyRetriever.RetrieveFromCommandLineOrEnvironment("seqLogAddress", "seqLogAddress", null);
                var seqApiKey = propertyRetriever.RetrieveFromCommandLineOrEnvironment("seqApiKey", "seqApiKey", null);
                manager.SetSeqLoggingLevel(propertyRetriever.RetrieveFromCommandLineOrEnvironment("seqMinimumLogEventLevel", "seqMinimumLogEventLevel", LogLevel.None));

                logger = logger
                    .WriteTo.Seq(
                        serverUrl: seqAddress,
                        apiKey: seqApiKey,
                        controlLevelSwitch: manager.SeqLoggingLevelSwitch);
            }

            return logger;
        }

        public static LoggerConfiguration WithFileLog(this LoggerConfiguration logger, NanoLoggerManager manager, IPropertyRetriever propertyRetriever)
        {
            var withFileLog = propertyRetriever.CheckFromCommandLine("withFileLog");

            if (withFileLog)
            {
                var fileMessageTemplate = propertyRetriever.RetrieveFromCommandLineOrEnvironment("fileMessageTemplate", "fileMessageTemplate", "{Timestamp} [{LogLevel}] (MachineName: {MachineName}) (Thread: {ThreadId}) {Message} {Exception}{NewLine}");
                var fileRollingInterval = propertyRetriever.RetrieveFromCommandLineOrEnvironment("fileRollingInterval", "fileRollingInterval", RollingInterval.Hour);
                manager.SetFileLoggingLevel(propertyRetriever.RetrieveFromCommandLineOrEnvironment("fileMinimumLogEventLevel", "fileMinimumLogEventLevel", LogLevel.None));

                logger = logger
                    .WriteTo.File(
                        path: $"log\\log_{propertyRetriever.RetrieveServiceName()}_.txt",
                        levelSwitch: manager.FileLoggingLevelSwitch,
                        rollingInterval: fileRollingInterval,
                        outputTemplate: fileMessageTemplate,
                        shared: true);
            }

            return logger;
        }

        public static LoggerConfiguration WithConsoleLog(this LoggerConfiguration logger, NanoLoggerManager manager, IPropertyRetriever propertyRetriever, bool withDefaultConsoleLog = false)
        {
            if (withDefaultConsoleLog)
            {
                logger = logger
                    .WriteTo.Console(
                        outputTemplate: "{Timestamp} [{ColoredLogLevel}] (MachineName: {MachineName}) (Thread: {ThreadId}) {Message} {Exception}{NewLine}",
                        levelSwitch: new LoggingLevelSwitch());
            }
            else
            {
                var withConsoleLog = propertyRetriever.CheckFromCommandLine("withConsoleLog");

                if (withConsoleLog)
                {
                    var consoleMessageTemplate = propertyRetriever.RetrieveFromCommandLineOrEnvironment("consoleMessageTemplate", "consoleMessageTemplate", "{Timestamp} [{LogLevel}] (MachineName: {MachineName}) (Thread: {ThreadId}) {Message} {Exception}{NewLine}").Replace("{LogLevel}", "{ColoredLogLevel}");
                    manager.SetConsoleLoggingLevel(propertyRetriever.RetrieveFromCommandLineOrEnvironment("consoleMinimumLogEventLevel", "consoleMinimumLogEventLevel", LogLevel.None));
                    logger = logger
                        .WriteTo.Console(
                            outputTemplate: consoleMessageTemplate,
                            levelSwitch: manager.ConsoleLoggingLevelSwitch);
                }
            }

            return logger;
        }

        public static LoggerConfiguration GetLoggerConfiguration(
            NanoLoggerManager manager,
            IPropertyRetriever propertyRetriever,
            bool withDefaultConsoleLog = false)
        {
            var logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithLoggerLevel()
                .MinimumLevel.Verbose();

            logger = logger
                .WithSeqLog(manager, propertyRetriever)
                .WithFileLog(manager, propertyRetriever)
                .WithConsoleLog(manager, propertyRetriever, withDefaultConsoleLog);

            return logger;
        }
    }
}
