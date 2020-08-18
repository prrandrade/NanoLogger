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
            Console.WriteLine("Now critical at console and warning at file!");
            loggerManager.SetConsoleLoggingLevel(LogLevel.Critical);
            loggerManager.SetFileLoggingLevel(LogLevel.Warning);

            Console.ReadKey(true);
            Console.WriteLine("Now at information at console and nothing at file!");
            loggerManager.SetConsoleLoggingLevel(LogLevel.Information);
            loggerManager.SetFileLoggingLevel(LogLevel.None);

            Console.ReadKey(true);
            Console.WriteLine("Now at warning at console and trace at file!");
            loggerManager.SetConsoleLoggingLevel(LogLevel.Warning);
            loggerManager.SetFileLoggingLevel(LogLevel.Trace);

            Console.ReadKey(true);

        }
    }
}
