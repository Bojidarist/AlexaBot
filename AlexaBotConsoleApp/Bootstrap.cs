using AlexaBotConsoleApp.Helpers;
using AlexaBotConsoleApp.Loggers;
using AlexaBotConsoleApp.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlexaBotConsoleApp
{
    public class Bootstrap
    {
        #region Private members

        // The client
        private DiscordSocketClient _client;

        // The environment variable with the bot token
        private readonly string tokenVar = "AlexaBotToken";

        /// <summary>
        /// Local logger
        /// </summary>
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Bootstrap(ILogger logger) { _logger = logger; }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the bot
        /// </summary>
        public async Task StartAsync()
        {
            Initializer.InitPaths();
            _logger.LogInfo("Initialized paths");

            using var services = ConfigureServices();
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = services.GetRequiredService<DiscordSocketClient>();

            _client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            // Tokens should be considered secret data and never hard-coded.
            // We can read from the environment variable to avoid hard-coding.
            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable(tokenVar, EnvironmentVariableTarget.User));
            await _client.StartAsync();

            // Here we initialize the logic required to register our commands.
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            // Make sure the program does not close
            await Task.Delay(-1);
        }

        /// <summary>
        /// Configures all services
        /// </summary>
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<AudioService>()
                .AddSingleton<VoiceRecordingService>()
                .AddSingleton<ILogger>(new ConsoleLogger())
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }

        /// <summary>
        /// Fires when a log occurs
        /// </summary>
        /// <param name="log">The log</param>
        private Task LogAsync(LogMessage log)
        {
            _logger.Log(log);
            return Task.CompletedTask;
        }

        #endregion
    }
}
