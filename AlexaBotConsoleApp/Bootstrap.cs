using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace AlexaBotConsoleApp
{
    public class Bootstrap
    {
        #region Private members

        // The bot client
        private readonly DiscordSocketClient _client;
        // The environment variable with the bot token
        private readonly string tokenVar = "AlexaBotToken";

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Bootstrap()
        {
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the bot
        /// </summary>
        public async Task StartAsync()
        {
            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable(tokenVar, EnvironmentVariableTarget.User));
            await _client.StartAsync();

            // Block the program until it is closed
            await Task.Delay(-1);
        }

        #endregion

        #region Tasks

        /// <summary>
        /// Fires when a message is received in a channel where the bot is connected
        /// </summary>
        /// <param name="msg">The message</param>
        private async Task MessageReceivedAsync(SocketMessage msg)
        {
            if (msg.Author.Id == _client.CurrentUser.Id)
            {
                return;
            }

            if (msg.Content == "!ping")
            {
                await msg.Channel.SendMessageAsync("Pong!");
            }
        }

        /// <summary>
        /// Fires when the bot is ready
        /// </summary>
        private Task ReadyAsync()
        {
            Console.WriteLine($"Client: { _client.CurrentUser } is connected!");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Fires when a log occurs
        /// </summary>
        /// <param name="log">The log</param>
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log);

            return Task.CompletedTask;
        }

        #endregion
    }
}
