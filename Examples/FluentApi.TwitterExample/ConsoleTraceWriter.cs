using System;
using Salience.FluentApi;

namespace FluentApi.TwitterExample
{
    class ConsoleTraceWriter : ITraceWriter
    {
        public void Trace(TraceLevel level, string message, Exception exception = null)
        {
            Console.WriteLine("[{0}] {1}", level, message);
        }
    }
}