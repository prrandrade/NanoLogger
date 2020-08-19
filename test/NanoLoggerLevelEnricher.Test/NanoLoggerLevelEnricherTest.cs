namespace NanoLoggerLevelEnricher.Test
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Parsing;
    using Xunit;

    public class NanoLoggerLevelEnricherTest
    {
        [Theory]
        [InlineData(LogEventLevel.Verbose, "\"TRC\"", "\"\x1b[38;5;0007mTRC\"")]
        [InlineData(LogEventLevel.Debug, "\"DBG\"", "\"\x1b[38;5;0007mDBG\"")]
        [InlineData(LogEventLevel.Information, "\"INF\"", "\"\x1b[38;2;0;150;0mINF\"")]
        [InlineData(LogEventLevel.Warning, "\"WRN\"", "\"\x1b[38;2;200;200;0mWRN\"")]
        [InlineData(LogEventLevel.Error, "\"ERR\"", "\"\x1b[38;2;255;0;0mERR\"")]
        [InlineData(LogEventLevel.Fatal, "\"CRT\"", "\"\x1b[38;2;255;255;255m\x1b[48;2;255;0;0mCRT\"")]
        public void Enrich(LogEventLevel level, string expectedLevel, string expectedColoredLevel)
        {
            // arrange
            var logEventPropertyFactory = new Mock<ILogEventPropertyFactory>();
            logEventPropertyFactory
                .Setup(x => x.CreateProperty(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
                .Returns<string, object, bool>((s,o,b) => new LogEventProperty(s, new ScalarValue(o)));

            var logEvent = new LogEvent(DateTimeOffset.Now, level, null, new MessageTemplate("", new List<MessageTemplateToken>()), new List<LogEventProperty>());
            var enricher = new NanoLoggerLevelEnricher();

            // act
            enricher.Enrich(logEvent, logEventPropertyFactory.Object);

            // assert
            Assert.Equal(expectedLevel, logEvent.Properties["LogLevel"].ToString());
            Assert.Equal(expectedColoredLevel, logEvent.Properties["ColoredLogLevel"].ToString());
        }
    }
}
