using System;

namespace Salience.FluentApi.TraceWriters
{
    /// <summary>
    /// Outputs traces in the Console.
    /// </summary>
    /// <remarks>The foreground color will depend on the trace level.</remarks>
    public class ConsoleTraceWriter : ITraceWriter
    {
        public void Trace(TraceLevel level, Exception exception, string message, params object[] args)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            ConsoleColor newColor = oldColor;
            if (level == TraceLevel.Debug)
                newColor = ConsoleColor.DarkGray;
            else if (level == TraceLevel.Warning)
                newColor = ConsoleColor.Yellow;
            else if (level == TraceLevel.Error || level == TraceLevel.Fatal)
                newColor = ConsoleColor.Red;

            Console.ForegroundColor = newColor;
            Console.Write("[{0}] ", level);
            Console.WriteLine(message, args);
            Console.ForegroundColor = oldColor;
        }
    }
}