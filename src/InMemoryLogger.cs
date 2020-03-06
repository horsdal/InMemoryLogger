namespace InMemLogger
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Logging;

  public static class LoggingBuilderExtensions
  {
    public static ILoggingBuilder AddInMemory(this ILoggingBuilder builder)
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

    public ILogger CreateLogger(string categoryName) => this.logger;

    public void Dispose() { }
  }

  public class InMemoryLogger : ILogger
  {
    private readonly List<(LogLevel, Exception, string)> logLines = new List<(LogLevel, Exception, string)>();

    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedLogs { get { lock (this.logLines) { return this.logLines.ToList(); } } }
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedTraceLogs { get { lock (this.logLines) { return this.logLines.Where(l => l.Item1 == LogLevel.Trace).ToList(); } } }
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedDebugLogs { get { lock (this.logLines) { return this.logLines.Where(l => l.Item1 == LogLevel.Debug).ToList(); } } }
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedInformationLogs { get { lock (this.logLines) { return this.logLines.Where(l => l.Item1 == LogLevel.Information).ToList(); } } }
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedWarningLogs { get { lock (this.logLines) { return this.logLines.Where(l => l.Item1 == LogLevel.Warning).ToList(); } } }
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedErrorLogs { get { lock (this.logLines) { return this.logLines.Where(l => l.Item1 == LogLevel.Error).ToList(); } } }
    public IEnumerable<(LogLevel Level, Exception Exception, string Message)> RecordedCriticalLogs { get { lock (this.logLines) { return this.logLines.Where(l => l.Item1 == LogLevel.Critical).ToList(); } } }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
      var message=formatter(state, exception);
      lock (this.logLines)
      {
        this.logLines.Add((logLevel, exception,message ));
      }
      if (logLevel>=MinimumEventSeverity)
      {
        LoggedEvent?.Invoke(this,(logLevel, exception,message ));
      }
    }
    /// <summary>
    /// This event handler is called synchronously inside the logging call.  Take great care when
    /// setting the minimum event severity low, or taking significant time within the event handler
    /// </summary>
    public LogLevel MinimumEventSeverity { get; set; } = LogLevel.None;
    public event EventHandler<(LogLevel Level, Exception Exception, string Message)> LoggedEvent;
  }
}
