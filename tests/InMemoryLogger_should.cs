namespace InMemLogger.Tests
{
  using Microsoft.Extensions.Logging;
  using System;
  using System.Linq;
  using System.Collections.Generic;
  using Xunit;
  using Microsoft.Extensions.DependencyInjection;

  public class InMemoryLogger_should
  {
    private readonly ILogger<InMemoryLogger_should> ilogger;
    private readonly InMemoryLogger inMemLogger;

    public static IEnumerable<object[]> LogLevels() =>
      ((IEnumerable<LogLevel>) Enum.GetValues(typeof(LogLevel))).Select(v => new object[] { v });

    public InMemoryLogger_should()
    {
      var services = new ServiceCollection()
        .AddLogging(x => x.SetMinimumLevel(LogLevel.Trace).AddInMemory())
        .BuildServiceProvider();

      this.ilogger = services.GetService<ILogger<InMemoryLogger_should>>();
      this.inMemLogger = services.GetService<InMemoryLogger>();
    }

    [Theory]
    [MemberData(nameof(LogLevels))]
    public void record_log_messages(LogLevel level)
    {
      const string expected = "testing";
      this.ilogger.Log(level, expected);

      Assert.Contains(this.inMemLogger.RecordedLogs, l => l.Level == level && l.Message == expected);
    }

    [Theory]
    [MemberData(nameof(LogLevels))]
    public void record_exceptions(LogLevel level)
    {
      var expected = new Exception();
      this.ilogger.Log(level, expected, "");

      Assert.Contains(this.inMemLogger.RecordedLogs, l => l.Level == level && l.Exception == expected);
    }
  }
}
