namespace NanoLogger
{
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
        public static IServiceCollection AddNanoLogger(this IServiceCollection @this)
        {
            @this.AddPropertyRetriever();

            bool withSeqLog, withConsoleLog, withFileLog;
            string serviceName;

            var seqControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var fileControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var consoleControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);

            var manager = new NanoLoggerManager(seqControlLevelSwitch, fileControlLevelSwitch, consoleControlLevelSwitch);
            @this.AddSingleton<INanoLoggerManager>(manager);

            #region Options for seq log

            string seqAddress = null;
            string seqApiKey = null;

            #endregion

            #region Options for file log

            var fileRollingInterval = RollingInterval.Hour;

            #endregion

            #region Options for console log



            #endregion

            using (var scope = @this.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                var propertyRetriever = scope.ServiceProvider.GetService<IPropertyRetriever>();

                serviceName = propertyRetriever.RetrieveServiceName();

                withSeqLog = propertyRetriever.CheckFromCommandLine("withSeqLog");
                withConsoleLog = propertyRetriever.CheckFromCommandLine("withConsoleLog");
                withFileLog = propertyRetriever.CheckFromCommandLine("withFileLog");

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
                    fileRollingInterval = propertyRetriever.RetrieveFromCommandLineOrEnvironment("fileRollingInterval", "fileRollingInterval", RollingInterval.Hour);
                    manager.SetFileLoggingLevel(propertyRetriever.RetrieveFromCommandLineOrEnvironment("fileMinimumLogEventLevel", "fileMinimumLogEventLevel", LogLevel.None));
                }

                #endregion

                #region Retrieving options for console log

                if (withConsoleLog)
                {
                    manager.SetConsoleLoggingLevel(propertyRetriever.RetrieveFromCommandLineOrEnvironment("consoleMinimumLogEventLevel", "consoleMinimumLogEventLevel", LogLevel.None));
                }

                #endregion
            }

            var logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .MinimumLevel.Verbose();

            if (withSeqLog)
            {
                logger = logger
                    .WriteTo
                    .Seq(seqAddress, apiKey: seqApiKey, controlLevelSwitch: seqControlLevelSwitch);
            }

            if (withFileLog)
            {
                logger = logger
                    .WriteTo
                    .File(path: $"log\\log_{serviceName}_.txt",
                        levelSwitch: fileControlLevelSwitch,
                        rollingInterval: fileRollingInterval,
                        shared: true);
            }

            if (withConsoleLog)
            {
                logger = logger
                    .WriteTo
                    .Console(levelSwitch: consoleControlLevelSwitch);
            }

            @this.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(logger.CreateLogger());
            });

            return @this;
        }
    }
}
