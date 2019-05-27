namespace InMemLogger
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Logging;

  public static class LoggingBuilderExtensions
  {
    public static ILoggingBuilder AddInMemmory(this ILoggingBuilder builder)
    {
      var logger = new InMemoryLogger();
      builder.Services.AddSingleton(logger);
      return builder.AddProvider(new InMemLoggerProvider(logger));
    }
  }

  public class InMemLoggerProvider : ILoggerProvider
  {
    private readonly InMemoryLogger logger;

    public InMemLoggerProvider(InMemoryLogger logger) => this.logger = logger;

    public ILogger CreateLogger(string categoryName) => logger;

    public void Dispose() { }
  }

  public class InMemoryLogger : ILogger
  {
    private readonly List<(LogLevel, Exception, string)> logLines = new List<(LogLevel, Exception, string)>();

    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedLogs => this.logLines.AsReadOnly();
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedTraceLogs => this.logLines.Where(l => l.Item1 == LogLevel.Trace);
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedDebugLogs => this.logLines.Where(l => l.Item1 == LogLevel.Debug);
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedInformationLogs => this.logLines.Where(l => l.Item1 == LogLevel.Information);
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedWarningLogs => this.logLines.Where(l => l.Item1 == LogLevel.Warning);
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedErrorLogs => this.logLines.Where(l => l.Item1 == LogLevel.Error);
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedCriticalLogs => this.logLines.Where(l => l.Item1 == LogLevel.Critical);

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
      this.logLines.Add((logLevel, exception, formatter(state, exception)));
    }
  }
}
