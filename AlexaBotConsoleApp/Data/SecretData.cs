using System;

namespace AlexaBotConsoleApp.Data
{
    public static class SecretData
    {
        /// <summary>
        /// The secret token that authenticates the bot
        /// </summary>
        public static string AlexaBotToken => Environment.GetEnvironmentVariable("AlexaBotToken", EnvironmentVariableTarget.User);

        /// <summary>
        /// The Azure Cognitive Services Speech Token
        /// </summary>
        public static string AzureSpeechToken => Environment.GetEnvironmentVariable("AzureSpeechToken", EnvironmentVariableTarget.User);

        /// <summary>
        /// The Azure Cognitive Services Speech Endpoint
        /// </summary>
        public static string AzureSpeechEndpoint => Environment.GetEnvironmentVariable("AzureSpeechEndpoint", EnvironmentVariableTarget.User);
    }
}
