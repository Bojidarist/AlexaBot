namespace AlexaBotConsoleApp.Loggers
{
    public interface ILogger
    {
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
