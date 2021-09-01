using System;
using Microsoft.Extensions.Logging;

namespace Salience.FluentApi.TraceWriters
{
    /// <summary>
    /// Output traces using the provided <see cref="ILogger"/>.
    /// </summary>
    public class LoggerTraceWriter : ITraceWriter
    {
        private readonly ILogger _logger;

        public LoggerTraceWriter(ILogger logger)
        {
            _logger = logger;
        }

        public void Trace(TraceLevel level, Exception exception, string message, params object[] args)
        {
            var logLevel = ConvertLevel(level);

            if (exception != null)
                _logger.Log(logLevel, exception, message, args);
            else 
                _logger.Log(logLevel, message, args);
        }

        private static LogLevel ConvertLevel(TraceLevel level)
        {
            switch (level)
            {
                case TraceLevel.Debug: return LogLevel.Debug;
                case TraceLevel.Warning: return LogLevel.Warning;
                case TraceLevel.Error: return LogLevel.Error;
                case TraceLevel.Fatal: return LogLevel.Critical;
                default: return LogLevel.Information;
            }
        }
    }
}