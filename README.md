[![Build Status](https://chr-horsdal.visualstudio.com/InMemoryLogger/_apis/build/status/horsdal.InMemoryLogger)](https://chr-horsdal.visualstudio.com/InMemoryLogger/_build/latest?definitionId=2)
[![NuGet version](https://img.shields.io/nuget/v/InMemoryLogger.svg?style=flat)](https://www.nuget.org/packages/InMemoryLogger)

# InMemoryLogger
Microsoft.Extensions.Logging compatible logger for recording log messages during tests

## Getting started

Install from NuGet:
```
> dotnet add package InMemoryLogger 
```

Then add InMemoryLogger as the logger in your applications service collection:
```csharp
      var services = new ServiceCollection()
        .AddLogging(x => x.AddInMemmory())
        .BuildServiceProvider();

      var inMemLogger = services.GetService<InMemoryLogger>();
```
And then write a test that asserts on logs messages:
```csharp
      var logger = services.GetService<ILogger<MyTest>>();
      logger.LogWarning("This is a log message");
      Assert.Contains(inMemLogger.RecordedWarningLogs, l => l.Message == "This is a log message");
```
Or on logged exceptions:
```csharp
      var logger = services.GetService<ILogger<MyTest>>();
      var expected = new Exception();
      logger.LogError(expected, "This is another log message");
      Assert.Contains(inMemLogger.RecordedErrorLogs, l => l.Exception == expected);
```
That's it. All logs are in memory, all logs are recorded.