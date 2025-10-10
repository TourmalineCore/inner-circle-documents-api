using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging.EventLog;

namespace Api.Configuration;

public static class LoggingExtensions
{
  public static void AddAppLogging(this WebApplicationBuilder builder)
  {
    const string LoggingSectionKey = "Logging";
    var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    var logging = builder.Logging;

    if (isWindows)
    {
      logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
      logging.AddEventLog();
    }

    logging.AddConfiguration(builder.Configuration.GetSection(LoggingSectionKey));
    logging.AddSimpleConsole(opts =>
    {
      opts.SingleLine = true;
      opts.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
      opts.UseUtcTimestamp = true;
    });

    logging.AddDebug();
    logging.AddEventSourceLogger();

    logging.Configure(options =>
    {
      options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
        | ActivityTrackingOptions.TraceId
        | ActivityTrackingOptions.ParentId;
    });
  }
}
