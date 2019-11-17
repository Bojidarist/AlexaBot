using System;

namespace AlexaBotConsoleApp.Data
{
    public static class SecretData
    {
        /// <summary>
        /// The secret token that authenticates the bot
        /// </summary>
        public static string AlexaBotToken => Environment.GetEnvironmentVariable("AlexaBotToken", EnvironmentVariableTarget.User);
    }
}
