# NanoLogger

# Summary

- [Introduction](#introduction)
- [Nuget Packages](#nuget-packages)
- [Serilog Configurations](#serilog-configurations)
- [Console Output](#console-output)
- [File Output](#file-output)
- [Seq Output](#seq-output)
- [How to use it](#how-to-use-it)

# Introduction

The NanoLogger package abstracts a [Serilog implementation](https://serilog.net/) with command line and /or environment parameters. Currently, this package is compatible with [Console Output](https://github.com/serilog/serilog-sinks-console), [File Output](https://github.com/serilog/serilog-sinks-file) and/or [Seq Output](https://github.com/serilog/serilog-sinks-seq), using the [.NET Logger Levels](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-5.0). This package is already compiled using **.NET 5.0**.

# Nuget Package

- [NanoLogger](https://www.nuget.org/packages/NanoLogger/)

# Serilog Configuration

Every log output can also optionally have informations about thread id, exception details, process id, machine name and user name. There's no mistery, the NanoLogger package is simply using some Serilog Enrichers:

- [Serilog.Enrichers.Process](https://github.com/serilog/serilog-enrichers-process)
- [Serilog.Enrichers.Thread](https://github.com/serilog/serilog-enrichers-thread)
- [Serilog.Enrichers.Environment](https://github.com/serilog/serilog-enrichers-environment)
- [Serilog.Exceptions](https://github.com/RehanSaeed/Serilog.Exceptions)

All log outputs can be configured to only log entries with a minimum output, using the [.NET Core LogLevel Enum](https://docs.microsoft.com/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-3.1). There's a hierarchical order of which logs are shown when a minimum log is set:

- **None** will deactivate every log!
- **Critical** will save only **Critical** logs.
- **Error** will save Critical and Error logs.
- **Warning** will save **Critical**, **Error** and **Warning** logs.
- **Information** will save **Critical**, **Error**, **Warning**, and **Information** logs.
- **Debug** will save **Critical**, **Error**, **Warning**, **Information** and **Debug** logs.
- **Verbose** will save every log!

For log outputs compatible with message templates, the default message template is:

```
{Timestamp} [{LogLevel}] (MachineName: {MachineName}) (Thread: {ThreadId}) {Message} {Exception}{NewLine}
```

As you can see, almost all template parameters follow the [default Serilog output paramaters](https://github.com/serilog/serilog/wiki/Formatting-Output) - with one exception. The **{LogLevel}** output parameter will show the current .NET Core Log Level, instead of the current Serilog Log Level (there are differences, but the NanoLogger will make a automatic translation). For the console output, the **LogLevel** information will be shown with ANSI scheme colors (only works on Windows 10, though).

# Console Output

The console output is the simplest output. It can be activated with the command line parameter **--withConsoleLog**. You can change the minimum level that will be shown on the console using the parameter **--consoleMinimumLogEventLevel** or the environment variable **consoleMinimumLogEventLevel**. The accepted values are  **Trace**, **Debug**, **Information**, **Warning**, **Error**, **Critical** and **None** ([.NET LogLevel Enum values](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-5.0) and the default value is **None**. You can also change the message template that will be shown using the parameter **--consoleMessageTemplate** or the environment variable **consoleMessageTemplate**.

# File Output

The file output will generate a text file inside the folder **/log** and can be activated with the command line parameter **--withFileLog**. You can change the minimum level that will be saved in the file using the parameter **--fileMinimumLogEventLevel** or the environment variable **fileMinimumLogEventLevel**. The accepted values are  **Trace**, **Debug**, **Information**, **Warning**, **Error**, **Critical** and **None** ([.NET LogLevel Enum values](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-5.0)) and the default value is **None**.  You can also change the message template that will be used with the parameter **--fileMessageTemplate** or the environment variable **fileMessageTemplate**. You can also change the rolling interval to create new log files using the parameter **--fileRollingInterval** or the environment variable **fileRollingInterval**. The accepted values are **Infinite**, **Year**, **Month**, **Day**, **Hour** and **Minute** - the default value is **Hour**.

# Seq Output

The Seq output will send all logs for a [Seq Server](https://datalust.co/seq) and can be activated with the command line parameter **--withSeqLog**. You just need to pass the server address and the server API key using the parameters **--seqLogAddress** and **--seqApiKey** or the environment variables **seqLogAddress** and **seqApiKey**. You can change the minimum level that will be saved in the file using the parameter **--seqMinimumLogEventLevel** or the environment variable **seqMinimumLogEventLevel**. The accepted values are  **Trace**, **Debug**, **Information**, **Warning**, **Error**, **Critical** and **None** ([.NET LogLevel Enum values](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-5.0)) and the default value is **None**.

# How to use it

It's simple! After installing the package, use the extension method  `.AddNanoLogger()` to add the injected dependencies in a `ServiceCollection` object. The following example show how it can be activated:

```
var collection = new ServiceCollection().AddNanoLogger()
```

For a fast console log installation, the parameter `withDefaultConsoleLog` can be used with the `true` value. In this case, the minimum log level is **Information**.

```
var collection = new ServiceCollection().AddNanoLogger(true)
```

