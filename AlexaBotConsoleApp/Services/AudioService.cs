using AlexaBotConsoleApp.Loggers;
using Discord;
using Discord.Audio;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AlexaBotConsoleApp.Services
{
    public class AudioService
    {
        #region Private members

        /// <summary>
        /// Thread-safe dictionary containing the connected channels
        /// </summary>
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        /// <summary>
        /// A local logger
        /// </summary>
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">A local logger</param>
        public AudioService(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Joins audio channel
        /// </summary>
        /// <param name="guild">The guild the bot is connected to</param>
        /// <param name="target">The target voice channel</param>
        /// <param name="mChannel">The message channel that allows sending messages etc.</param>
        public async Task JoinAudio(IGuild guild, IVoiceChannel target, IMessageChannel mChannel = null)
        {
            if (target == null)
            {
                if (mChannel != null) { await mChannel.SendMessageAsync("You must be connected to a channel for me to join!"); }
                return;
            }
            if (ConnectedChannels.TryGetValue(guild.Id, out _)) { return; }
            if (target.Guild.Id != guild.Id) { return; }

            var audioClient = await target.ConnectAsync();

            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                _logger.LogInfo($"Connected to a voice channel in: { guild.Name }");
            }
        }

        /// <summary>
        /// Leaves audio channel
        /// </summary>
        /// <param name="guild">The guild the bot is connected to</param>
        public async Task LeaveAudio(IGuild guild)
        {
            if (ConnectedChannels.TryRemove(guild.Id, out IAudioClient client))
            {
                await client.StopAsync();
                client.Dispose();
                _logger.LogInfo($"Disconnected from a voice channel in: { guild.Name }");
            }
        }

        /// <summary>
        /// Sends audio to a voice channel
        /// </summary>
        /// <param name="guild">The guild the bot is connected to</param>
        /// <param name="channel">The voice channel the bot is connected to</param>
        /// <param name="path">The path to an audio file</param>
        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) { return; }
            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            if (!File.Exists(path))
            {
                _logger.LogError($"File: { path } does not exist!");
                return;
            }
            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                _logger.LogInfo($"Playing { path } to { guild.Name }");
                await channel.SendMessageAsync($"Playing { path }!");
                using var ffmpeg = CreateProcess(path);
                using var stream = client.CreatePCMStream(AudioApplication.Music);
                try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                finally
                {
                    await stream.FlushAsync();
                    _logger.LogInfo($"Finished playing { path } to { guild.Name }");
                }
            }
        }

        /// <summary>
        /// Creates a process for ffmpeg used for streaming audio
        /// </summary>
        /// <param name="path">Path to an audio file</param>
        private Process CreateProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        #endregion
    }
}
