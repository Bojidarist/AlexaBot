using AlexaBotConsoleApp.Data;
using AlexaBotConsoleApp.Loggers;
using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
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
        /// Thread-safe dictionary containing the running songs in different guilds and their path
        /// </summary>
        private readonly ConcurrentDictionary<ulong, string> RunningSongs = new ConcurrentDictionary<ulong, string>();

        /// <summary>
        /// A local logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// A local random number generator
        /// </summary>
        private readonly Random _randomGenerator = new Random();

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
            ClearRunningSongsInGuild(guild);
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
                using var ffmpeg = CreateProcess(path);
                using var stream = client.CreatePCMStream(AudioApplication.Music);
                try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                finally
                {
                    ffmpeg.Kill();
                    await stream.FlushAsync();
                    _logger.LogInfo($"Finished playing { path } to { guild.Name }");
                }
            }
        }

        /// <summary>
        /// Downloads a song using it's name or a link to it and plays it to a channel
        /// </summary>
        /// <param name="input">The name or link of the song</param>
        /// <param name="context">The information about the command</param>
        public async Task DownloadAndPlayAudioAsync(string input, ICommandContext context)
        {
            if (string.IsNullOrWhiteSpace(input)) { return; }
            if (!ConnectedChannels.TryGetValue(context.Guild.Id, out _)) { await JoinAudio(context.Guild, (context.User as IGuildUser).VoiceChannel, context.Channel); }

            Regex urlRegex = new Regex(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
            bool isURL = urlRegex.IsMatch(input);

            string songPath = $"{ Path.Combine(DataPaths.SongDownloadPath, $"{ _randomGenerator.Next(0, int.MaxValue).ToString() }.mp3") }";
            YoutubeDLAudio(input, songPath, isURL);
            if (File.Exists(songPath))
            {
                if (RunningSongs.TryAdd(context.Guild.Id, songPath))
                {
                    await SendAudioAsync(context.Guild, context.Channel, songPath);
                    File.Delete(songPath);
                    if (!RunningSongs.TryRemove(context.Guild.Id, out _))
                    {
                        _logger.LogWarning($"Unable to remove { input } from RunningSongs after the audio stream ended");
                    }
                }
                else
                {
                    File.Delete(songPath);
                    _logger.LogWarning($"Unable to add { input } to RunningSongs");
                }
            }
            else
            {
                _logger.LogInfo($"Song: { input } Requested by: { context.User.Username } From: { context.Guild.Name } did not download.");
            }
        }

        /// <summary>
        /// Clears all running songs from a specific guild
        /// </summary>
        /// <param name="guild">The guild</param>
        private void ClearRunningSongsInGuild(IGuild guild)
        {
            foreach (var item in RunningSongs)
            {
                if (guild.Id == item.Key)
                {
                    File.Delete(item.Value);
                    RunningSongs.TryRemove(item.Key, out _);
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

        /// <summary>
        /// Downloads audio from the Internet and stores it to a local file using 'youtube-dl'
        /// </summary>
        /// <param name="audio">The name or link to the audio</param>
        /// <param name="outputPath">The path the audio will be saved to</param>
        /// <param name="isLink">Indicates if the input is a link</param>
        private void YoutubeDLAudio(string audio, string outputPath, bool isLink)
        {
            string filter = "\"duration < 600\"";
            if (isLink)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "youtube-dl",
                    Arguments = $"-x --audio-format mp3 --match-filter { filter } { audio } -o { outputPath }",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }).WaitForExit();
            }
            else
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "youtube-dl",
                    Arguments = $"-x --audio-format mp3 --match-filter { filter } \"ytsearch1:{ audio }\" -o { outputPath }",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }).WaitForExit();
            }
        }

        #endregion
    }
}
