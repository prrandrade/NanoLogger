namespace NanoLogger.Test.Enrichers
{
    using System;
    using NanoLogger.Enrichers;
    using Serilog;
    using Serilog.Configuration;
    using Xunit;

    public class NanoLoggerLevelEnricherExtensions
    {
        [Fact]
        public void NanoLoggerLevelEnricher_Add()
        {
            // arrange
            var loggerConfiguration = new LoggerConfiguration();

            // act
            loggerConfiguration
                .Enrich.WithLoggerLevel()
                .CreateLogger();
        }

        [Fact]
        public void NanoLoggerLevelEnricher_Error()
        {
            // act
            var result = Record.Exception(() => ((LoggerEnrichmentConfiguration) null).WithLoggerLevel());

            // assert
            Assert.IsType<ArgumentNullException>(result);
            Assert.Equal("Value cannot be null. (Parameter 'enrichmentConfiguration')", result.Message);
        }
    }
}
