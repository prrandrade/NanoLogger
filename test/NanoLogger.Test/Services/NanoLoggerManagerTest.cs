namespace NanoLogger.Test.Services
{
    using System;
    using Microsoft.Extensions.Logging;
    using NanoLogger.Services;
    using Serilog.Core;
    using Serilog.Events;
    using Xunit;

    public class NanoLoggerManagerTest
    {
        [Theory]
        [InlineData(LogLevel.Trace, LogEventLevel.Verbose)]
        [InlineData(LogLevel.Debug, LogEventLevel.Debug)]
        [InlineData(LogLevel.Information, LogEventLevel.Information)]
        [InlineData(LogLevel.Warning, LogEventLevel.Warning)]
        [InlineData(LogLevel.Error, LogEventLevel.Error)]
        [InlineData(LogLevel.Critical, LogEventLevel.Fatal)]
        [InlineData(LogLevel.None, (LogEventLevel) 1 + (int) LogEventLevel.Fatal)]
        public void SetSeqLoggingLevel(LogLevel logLevel, LogEventLevel logEventLevel)
        {
            // arrange
            var seqLevel = new LoggingLevelSwitch();
            var fileLevel = new LoggingLevelSwitch();
            var consoleLevel = new LoggingLevelSwitch();

            var nanoLoggerManager = new NanoLoggerManager(seqLevel, fileLevel, consoleLevel);

            // act
            nanoLoggerManager.SetSeqLoggingLevel(logLevel);

            // assert
            Assert.Equal(logEventLevel, nanoLoggerManager.SeqLoggingLevelSwitch.MinimumLevel);
        }

        [Fact]
        public void SetSeqLoggingLevel_Error()
        {
            // arrange
            var seqLevel = new LoggingLevelSwitch();
            var fileLevel = new LoggingLevelSwitch();
            var consoleLevel = new LoggingLevelSwitch();

            var nanoLoggerManager = new NanoLoggerManager(seqLevel, fileLevel, consoleLevel);

            // act
            var result = Record.Exception(() => nanoLoggerManager.SetSeqLoggingLevel((LogLevel)8));

            // assert
            Assert.IsType<ArgumentOutOfRangeException>(result);
            Assert.Equal((LogLevel)8, ((ArgumentOutOfRangeException)result).ActualValue);
            Assert.Equal("logLevel", ((ArgumentOutOfRangeException)result).ParamName);
        }

        [Theory]
        [InlineData(LogLevel.Trace, LogEventLevel.Verbose)]
        [InlineData(LogLevel.Debug, LogEventLevel.Debug)]
        [InlineData(LogLevel.Information, LogEventLevel.Information)]
        [InlineData(LogLevel.Warning, LogEventLevel.Warning)]
        [InlineData(LogLevel.Error, LogEventLevel.Error)]
        [InlineData(LogLevel.Critical, LogEventLevel.Fatal)]
        [InlineData(LogLevel.None, (LogEventLevel) 1 + (int) LogEventLevel.Fatal)]
        public void SetFileLoggingLevel(LogLevel logLevel, LogEventLevel logEventLevel)
        {
            // arrange
            var seqLevel = new LoggingLevelSwitch();
            var fileLevel = new LoggingLevelSwitch();
            var consoleLevel = new LoggingLevelSwitch();

            var nanoLoggerManager = new NanoLoggerManager(seqLevel, fileLevel, consoleLevel);

            // act
            nanoLoggerManager.SetFileLoggingLevel(logLevel);

            // assert
            Assert.Equal(logEventLevel, nanoLoggerManager.FileLoggingLevelSwitch.MinimumLevel);
        }

        [Fact]
        public void SetFileLoggingLevel_Error()
        {
            // arrange
            var seqLevel = new LoggingLevelSwitch();
            var fileLevel = new LoggingLevelSwitch();
            var consoleLevel = new LoggingLevelSwitch();

            var nanoLoggerManager = new NanoLoggerManager(seqLevel, fileLevel, consoleLevel);

            // act
            var result = Record.Exception(() => nanoLoggerManager.SetFileLoggingLevel((LogLevel)8));

            // assert
            Assert.IsType<ArgumentOutOfRangeException>(result);
            Assert.Equal((LogLevel)8, ((ArgumentOutOfRangeException)result).ActualValue);
            Assert.Equal("logLevel", ((ArgumentOutOfRangeException)result).ParamName);
        }

        [Theory]
        [InlineData(LogLevel.Trace, LogEventLevel.Verbose)]
        [InlineData(LogLevel.Debug, LogEventLevel.Debug)]
        [InlineData(LogLevel.Information, LogEventLevel.Information)]
        [InlineData(LogLevel.Warning, LogEventLevel.Warning)]
        [InlineData(LogLevel.Error, LogEventLevel.Error)]
        [InlineData(LogLevel.Critical, LogEventLevel.Fatal)]
        [InlineData(LogLevel.None, (LogEventLevel) 1 + (int) LogEventLevel.Fatal)]
        public void SetConsoleLoggingLevel(LogLevel logLevel, LogEventLevel logEventLevel)
        {
            // arrange
            var seqLevel = new LoggingLevelSwitch();
            var fileLevel = new LoggingLevelSwitch();
            var consoleLevel = new LoggingLevelSwitch();

            var nanoLoggerManager = new NanoLoggerManager(seqLevel, fileLevel, consoleLevel);

            // act
            nanoLoggerManager.SetConsoleLoggingLevel(logLevel);

            // assert
            Assert.Equal(logEventLevel, nanoLoggerManager.ConsoleLoggingLevelSwitch.MinimumLevel);
        }

        [Fact]
        public void SetConsoleLoggingLevel_Error()
        {
            // arrange
            var seqLevel = new LoggingLevelSwitch();
            var fileLevel = new LoggingLevelSwitch();
            var consoleLevel = new LoggingLevelSwitch();

            var nanoLoggerManager = new NanoLoggerManager(seqLevel, fileLevel, consoleLevel);

            // act
            var result = Record.Exception(() => nanoLoggerManager.SetConsoleLoggingLevel((LogLevel)8));

            // assert
            Assert.IsType<ArgumentOutOfRangeException>(result);
            Assert.Equal((LogLevel)8, ((ArgumentOutOfRangeException)result).ActualValue);
            Assert.Equal("logLevel", ((ArgumentOutOfRangeException)result).ParamName);
        }
    }
}
