namespace AlexaBotConsoleApp.Loggers
{
    public interface ILogger
    {
        /// <summary>
        /// Log a uncategorized message
        /// </summary>
        /// <param name="message">The message</param>
        public void Log(object message);

        /// <summary>
        /// Log some info
        /// </summary>
        /// <param name="message">The info</param>
        public void LogInfo(object message);

        /// <summary>
        /// Log a warning
        /// </summary>
        /// <param name="message">The warning</param>
        public void LogWarning(object message);

        /// <summary>
        /// Log a error
        /// </summary>
        /// <param name="message">The error</param>
        public void LogError(object message);

        /// <summary>
        /// Log a fatal error
        /// </summary>
        /// <param name="message">The fatal error</param>
        public void LogFatal(object message);
    }
}
