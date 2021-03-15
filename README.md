<p align="center">
  <a href="https://github.com/AnderssonPeter/LoggerHealthCheck">
    <img src="icon.svg" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">LoggerHealthCheck</h3>

  <p align="center">
    Serve smaller files with zero-ish server overhead
    <br />
    <br />
    ·
    <a href="https://github.com/AnderssonPeter/LoggerHealthCheck/issues">Report Bug</a>
    ·
    <a href="https://github.com/AnderssonPeter/LoggerHealthCheck/issues">Request Feature</a>
  </p>
</p>


[![NuGet version](https://badge.fury.io/nu/LoggerHealthCheck.svg)](https://badge.fury.io/nu/LoggerHealthCheck)
[![run unit tests](https://github.com/AnderssonPeter/LoggerHealthCheck/workflows/run%20unit%20tests/badge.svg)](https://github.com/AnderssonPeter/LoggerHealthCheck/actions?query=workflow%3A%22run+unit+tests%22)
[![Coverage Status](https://coveralls.io/repos/github/AnderssonPeter/LoggerHealthCheck/badge.svg)](https://coveralls.io/github/AnderssonPeter/LoggerHealthCheck)
[![GitHub license](https://img.shields.io/badge/license-Apache%202-blue.svg)](https://raw.githubusercontent.com/AnderssonPeter/LoggerHealthCheck/master/LICENSE)


## Table of Contents
* [About the Project](#about-the-project)
* [Getting Started](#getting-started)
* [Example](#example)

## About The Project
Project allows you to have a HealthCheck that changes status based on if a Warning, Error or Critical event is logged.

## Getting Started

In `Startup.ConfigureServices` add
```c#
services.AddHealthChecks()
        .AddLoggerHealthCheck();
```
This will add a healtcheck named `Logs` that will have the status `Degraded` if any `Warning`s have been logged in the last 5 minutes, if any `Error`s or `Critical`s have been logged it will have the status `Unhealthy`.

### With Serilog
In `Program.cs` add
```c#
.ConfigureLogging(builder => builder.AddHealthCheckLogger())
.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration), writeToProviders: true)
```

### Without Serilog
In `Program.cs` add
```c#
.ConfigureLogging(builder => {
    builder.AddHealthCheckLogger();
})
```

### Customization

#### Healtcheck for specific class
If you want to add a specific HealthCheck for one class you can use the `.AddLoggerHealthCheckForType<T>()` method in `Startup.ConfigureServices` this will scan for log entries that either have source set as `T`, or where a Exception's stacktrace contains the type.

#### Custom filtration
Filtration can be done at two diffrent levels
1. At the Global level
When calling `AddHealthCheckLogger()` you can provide a [HealthCheckLoggerProviderConfiguration](LoggerHealthCheck/HealthCheckLoggerProviderConfiguration.cs) instance, this allows you to specify a custom filtration, you should allways include `Filters.DefaultGlobalFilter` otherwise you might get a endless loop.
2. At HealthCheck level
When calling `AddLoggerHealthCheck()` you can provide a [LoggerHealthCheckOptions](LoggerHealthCheck/LoggerHealthCheckOptions.cs) instance, this allows you to specify a specific filter for that specific HealthCheck.

#### Configuration

##### Global (AddHealthCheckLogger)
See [HealthCheckLoggerProviderConfiguration](LoggerHealthCheck/HealthCheckLoggerProviderConfiguration.cs)

#### HealthCheck specific (AddLoggerHealthCheck)
See [LoggerHealthCheckOptions](LoggerHealthCheck/LoggerHealthCheckOptions.cs)

## Example
A example can be found in the [LoggerHealthCheckExample](LoggerHealthCheckExample) and [LoggerHealthCheckExampleWithSerilog](LoggerHealthCheckExampleWithSerilog) directory.
