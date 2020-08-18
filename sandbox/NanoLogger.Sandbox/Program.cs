namespace NanoLogger.Sandbox
{
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
            logger.LogInformation("test!");
        }
    }
}
