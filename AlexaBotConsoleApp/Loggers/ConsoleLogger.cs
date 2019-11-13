using System;

namespace AlexaBotConsoleApp.Loggers
{
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Log a uncategorized message
        /// </summary>
        /// <param name="message">The message</param>
        public void Log(object message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"LOG: { message }");
            Console.ResetColor();
        }

        /// <summary>
        /// Log some info
        /// </summary>
        /// <param name="message">The info</param>
        public void LogInfo(object message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"INFO: { message }");
            Console.ResetColor();
        }

        /// <summary>
        /// Log a warning
        /// </summary>
        /// <param name="message">The warning</param>
        public void LogWarning(object message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARNING: { message }");
            Console.ResetColor();
        }

        /// <summary>
        /// Log a error
        /// </summary>
        /// <param name="message">The error</param>
        public void LogError(object message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: { message }");
            Console.ResetColor();
        }

        /// <summary>
        /// Log a fatal error
        /// </summary>
        /// <param name="message">The fatal error</param>
        public void LogFatal(object message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"FATAL: { message }");
            Console.ResetColor();
        }
    }
}
