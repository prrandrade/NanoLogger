namespace NanoLogger.Sandbox
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    internal class Program
    {
        private static void Main()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddNanoLogger();
            var provider = serviceCollection.BuildServiceProvider();
            
            var logger = provider.GetService<ILogger<Program>>();
            var loggerManager = provider.GetService<INanoLoggerManager>();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    logger.LogTrace("Trace!");
                    logger.LogDebug("Debug!");
                    logger.LogInformation("Information!");
                    logger.LogWarning("Warning!");
                    logger.LogError("Error!");
                    logger.LogCritical("Critical!");
                    Thread.Sleep(1000);
                }
            });

            Console.ReadKey(true);
            loggerManager.SetConsoleLoggingLevel(LogLevel.Trace);
            loggerManager.SetFileLoggingLevel(LogLevel.Trace);

            Console.ReadKey(true);

        }
    }
}
