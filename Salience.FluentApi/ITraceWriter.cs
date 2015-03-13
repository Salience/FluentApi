using System;

namespace Salience.FluentApi
{
    public enum TraceLevel { Debug, Info, Warning, Error, Fatal }

    public interface ITraceWriter
    {
        /// <summary>
        /// Writes a trace message.
        /// </summary>
        void Trace(TraceLevel level, string message, Exception exception = null);
    }
}