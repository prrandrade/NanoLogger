namespace NanoLogger
{
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using PropertyRetriever;
    using PropertyRetriever.Interfaces;
    using Serilog;
    using Serilog.Exceptions;

    public static class NanoLoggerExtensions
    {
        public static IServiceCollection AddNanoLogger(this IServiceCollection @this)
        {
            @this.AddPropertyRetriever();

            bool withSeqLog, withConsoleLog, withFileLog;
            string serviceName, seqAddress, seqApiKey;


            #region Options for file log

            RollingInterval fileRollingInterval;


            #endregion

            using (var scope = @this.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                var propertyRetriever = scope.ServiceProvider.GetService<IPropertyRetriever>();

                serviceName = propertyRetriever.RetrieveServiceName();

                seqAddress = propertyRetriever.RetrieveFromEnvironment<string>("seqLogAddress", null);
                seqApiKey = propertyRetriever.RetrieveFromEnvironment<string>("seqApiKey", null);

                withSeqLog = propertyRetriever.CheckFromCommandLine("withSeqLog");
                withConsoleLog = propertyRetriever.CheckFromCommandLine("withConsoleLog");
                withFileLog = propertyRetriever.CheckFromCommandLine("withFileLog");

                #region Retrieving options for file log

                fileRollingInterval = propertyRetriever.RetrieveFromCommandLine<RollingInterval>("fileRollingInterval").ToList()[0];

                #endregion
            }

            var logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId();

            if (withSeqLog && !string.IsNullOrWhiteSpace(seqAddress) && !string.IsNullOrWhiteSpace(seqApiKey))
                logger = logger.WriteTo.Seq(serverUrl: seqAddress, apiKey: seqApiKey);

            if (withFileLog)
                logger = logger.WriteTo.File(path: $"log\\log_{serviceName}_.txt", rollingInterval: fileRollingInterval, shared: true);

            if (withConsoleLog)
                logger = logger.WriteTo.Console();

            Log.Logger = logger.CreateLogger();

            @this.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });
            return @this;
        }
    }
}
