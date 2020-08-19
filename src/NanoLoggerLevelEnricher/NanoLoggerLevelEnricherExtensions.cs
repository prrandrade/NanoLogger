namespace NanoLoggerLevelEnricher
{
    using System;
    using Serilog;
    using Serilog.Configuration;

    /// <summary>
    /// Extends <see cref="T:Serilog.LoggerConfiguration" /> to add <see cref="Microsoft.Extensions.Logging.LogLevel"/> info.
    /// </summary>
    public static class NanoLoggerLevelEnricherExtensions
    {
        public static LoggerConfiguration WithLoggerLevel(this LoggerEnrichmentConfiguration enrichmentConfiguration) 
            => enrichmentConfiguration != null ? enrichmentConfiguration.With<NanoLoggerLevelEnricher>() : throw new ArgumentNullException(nameof (enrichmentConfiguration));
    }
}
