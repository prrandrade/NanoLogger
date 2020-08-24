namespace NanoLogger.Test
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NanoLogger.Services;
    using NanoLoggerLevelEnricher;
    using PropertyRetriever.Interfaces;
    using Serilog;
    using Serilog.Core;
    using Serilog.Enrichers;
    using Serilog.Events;
    using Serilog.Exceptions.Core;
    using Xunit;

    public class NanoLoggerExtensionsTest
    {
        [Fact]
        public void GetLoggerConfigurationTest_BasicConfiguration()
        {
            // arrange
            var propertyRetriever = Mock.Of<IPropertyRetriever>();
            var seqControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var fileControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var consoleControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var manager = new NanoLoggerManager(seqControlLevelSwitch, fileControlLevelSwitch, consoleControlLevelSwitch);

            // act
            var logger = NanoLoggerExtensions.GetLoggerConfiguration(manager, propertyRetriever);

            // assert
            var enrichersField = typeof(LoggerConfiguration).GetField("_enrichers", BindingFlags.NonPublic | BindingFlags.Instance);
            var enrichersValue = (List<ILogEventEnricher>)enrichersField.GetValue(logger);

            Assert.Equal(5, enrichersValue.Count);
            Assert.IsType<ExceptionEnricher>(enrichersValue[0]);
            Assert.IsType<MachineNameEnricher>(enrichersValue[1]);
            Assert.IsType<ProcessIdEnricher>(enrichersValue[2]);
            Assert.IsType<ThreadIdEnricher>(enrichersValue[3]);
            Assert.IsType<NanoLoggerLevelEnricher>(enrichersValue[4]);

            var minimumLevelField = typeof(LoggerConfiguration).GetField("_minimumLevel", BindingFlags.NonPublic | BindingFlags.Instance);
            var minimumLevelValue = (LogEventLevel)minimumLevelField.GetValue(logger);
            Assert.Equal(LogEventLevel.Verbose, minimumLevelValue);
        }

        [Fact]
        public void GetLoggerConfigurationTest_SeqOutput()
        {
            // arrange
            const string seqAddress = "http://seq-address/";
            const string seqApiKey = "seq-api-key";
            const LogLevel logLevel = LogLevel.Critical;

            var propertyRetriever = Mock.Of<IPropertyRetriever>();
            Mock.Get(propertyRetriever).Setup(x => x.CheckFromCommandLine("withSeqLog")).Returns(true);
            Mock.Get(propertyRetriever).Setup(x => x.RetrieveFromCommandLineOrEnvironment("seqLogAddress", "seqLogAddress", null)).Returns(seqAddress);
            Mock.Get(propertyRetriever).Setup(x => x.RetrieveFromCommandLineOrEnvironment("seqApiKey", "seqApiKey", null)).Returns(seqApiKey);
            Mock.Get(propertyRetriever).Setup(x => x.RetrieveFromCommandLineOrEnvironment("seqMinimumLogEventLevel", "seqMinimumLogEventLevel", LogLevel.None)).Returns(logLevel);

            var seqControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var manager = new NanoLoggerManager(seqControlLevelSwitch, null, null);

            // act
            var logger = NanoLoggerExtensions.GetLoggerConfiguration(manager, propertyRetriever);

            // assert
            var sinksField = typeof(LoggerConfiguration).GetField("_logEventSinks", BindingFlags.NonPublic | BindingFlags.Instance);
            var sinksValue = (List<ILogEventSink>) sinksField.GetValue(logger);
            var sinkSeq = sinksValue[0];
            var sinkSeqAssembly = Assembly.Load("Serilog.Sinks.Seq");
            var sinkSeqType = sinkSeqAssembly.GetType("Serilog.Sinks.Seq.SeqSink");

            var seqApiField = sinkSeqType.GetField("_apiKey", BindingFlags.NonPublic | BindingFlags.Instance);
            var seqApiValue = (string) seqApiField.GetValue(sinkSeq);
            Assert.Equal(seqApiKey, seqApiValue);

            var seqHttpClientField = sinkSeqType.GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Instance);
            var seqhttpClientValue = (HttpClient)seqHttpClientField.GetValue(sinkSeq);
            Assert.Equal(seqAddress, seqhttpClientValue.BaseAddress.AbsoluteUri);

            var seqControlledSwitchField = sinkSeqType.GetField("_controlledSwitch", BindingFlags.NonPublic | BindingFlags.Instance);
            var seqControlledSwitchValue = seqControlledSwitchField.GetValue(sinkSeq);
            var seqLoggingLevelSwitchField = seqControlledSwitchValue.GetType().GetField("_controlledSwitch", BindingFlags.NonPublic | BindingFlags.Instance);
            var seqLoggingLevelSwitchValue = seqLoggingLevelSwitchField.GetValue(seqControlledSwitchValue);

            Assert.Equal(manager.SeqLoggingLevelSwitch, seqLoggingLevelSwitchValue);
        }

        [Fact]
        public void GetLoggerConfigurationTest_FileOutput()
        {
            // arrange
            const LogLevel logLevel = LogLevel.Information;

            var propertyRetriever = Mock.Of<IPropertyRetriever>();
            Mock.Get(propertyRetriever).Setup(x => x.CheckFromCommandLine("withFileLog")).Returns(true);
            Mock.Get(propertyRetriever).Setup(x => x.RetrieveFromCommandLineOrEnvironment("fileMinimumLogEventLevel", "fileMinimumLogEventLevel", LogLevel.None)).Returns(logLevel);
            Mock.Get(propertyRetriever).Setup(x => x.RetrieveFromCommandLineOrEnvironment("fileMessageTemplate", "fileMessageTemplate", It.IsAny<string>())).Returns((string s1, string s2, string s3) => s3);

            var fileControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var manager = new NanoLoggerManager(null, fileControlLevelSwitch, null);

            // act
            var logger = NanoLoggerExtensions.GetLoggerConfiguration(manager, propertyRetriever);

            // assert
            var sinksField = typeof(LoggerConfiguration).GetField("_logEventSinks", BindingFlags.NonPublic | BindingFlags.Instance);
            var sinksValue = (List<ILogEventSink>) sinksField.GetValue(logger);
            var sinkFile = sinksValue[0];
            var sinkFileAssembly = Assembly.Load("Serilog");
            var sinkFileType = sinkFileAssembly.GetType("Serilog.Core.Sinks.RestrictedSink");
            
            var fileControlledSwitchField = sinkFileType.GetField("_levelSwitch", BindingFlags.NonPublic | BindingFlags.Instance);
            var fileControlledSwitchValue = fileControlledSwitchField.GetValue(sinkFile);
            Assert.Equal(manager.FileLoggingLevelSwitch, (LoggingLevelSwitch)fileControlledSwitchValue);
        }

        [Fact]
        public void GetLoggerConfigurationTest_ConsoleOutput()
        {
            // arrange
            const LogLevel logLevel = LogLevel.Warning;

            var propertyRetriever = Mock.Of<IPropertyRetriever>();
            Mock.Get(propertyRetriever).Setup(x => x.CheckFromCommandLine("withConsoleLog")).Returns(true);
            Mock.Get(propertyRetriever).Setup(x => x.RetrieveFromCommandLineOrEnvironment("consoleMinimumLogEventLevel", "consoleMinimumLogEventLevel", LogLevel.None)).Returns(logLevel);
            Mock.Get(propertyRetriever).Setup(x => x.RetrieveFromCommandLineOrEnvironment("consoleMessageTemplate", "consoleMessageTemplate", It.IsAny<string>())).Returns((string s1, string s2, string s3) => s3);

            var consoleControlLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
            var manager = new NanoLoggerManager(null, null, consoleControlLevelSwitch);

            // act
            var logger = NanoLoggerExtensions.GetLoggerConfiguration(manager, propertyRetriever);

            // assert
            var sinksField = typeof(LoggerConfiguration).GetField("_logEventSinks", BindingFlags.NonPublic | BindingFlags.Instance);
            var sinksValue = (List<ILogEventSink>) sinksField.GetValue(logger);
            var sinkConsole = sinksValue[0];
            var sinkConsoleAssembly = Assembly.Load("Serilog");
            var sinkConsoleType = sinkConsoleAssembly.GetType("Serilog.Core.Sinks.RestrictedSink");
            
            var consoleControlledSwitchField = sinkConsoleType.GetField("_levelSwitch", BindingFlags.NonPublic | BindingFlags.Instance);
            var consoleControlledSwitchValue = consoleControlledSwitchField.GetValue(sinkConsole);
            Assert.Equal(manager.ConsoleLoggingLevelSwitch, (LoggingLevelSwitch)consoleControlledSwitchValue);
        }

        [Fact]
        public void AddNanoLogger()
        {
            // arrange
            var services = new ServiceCollection();

            // act
            services.AddNanoLogger();
            var container = services.BuildServiceProvider();

            // assert
            var logger = container.GetService<ILogger<NanoLoggerExtensionsTest>>();
            Assert.NotNull(logger);
        }
    }
}
