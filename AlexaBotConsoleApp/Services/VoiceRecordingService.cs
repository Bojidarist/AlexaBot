using AlexaBotConsoleApp.Data;
using AlexaBotConsoleApp.Helpers;
using AlexaBotConsoleApp.Loggers;
using AlexaBotConsoleApp.Models;
using Discord;
using Discord.Audio.Streams;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AlexaBotConsoleApp.Services
{
    public class VoiceRecordingService
    {
        #region Private members

        /// <summary>
        /// A local logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// A local <see cref="AzureSpeechService"/>
        /// </summary>
        private readonly AzureSpeechService _azureSpeech;

        /// <summary>
        /// A local <see cref="AudioService"/>
        /// </summary>
        private readonly AudioService _audioService;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">A local logger</param>
        /// <param name="audioService">A local audio service</param>
        /// <param name="azureSpeechService">A local azure speech service</param>
        public VoiceRecordingService(ILogger logger, AzureSpeechService azureSpeechService, AudioService audioService)
        {
            _logger = logger;
            _azureSpeech = azureSpeechService;
            _audioService = audioService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Listens to a user and records the audio
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="context">Information about the command</param>
        public async Task ListenUserAsync(IGuildUser user, ICommandContext context)
        {
            var socketUser = (user as SocketGuildUser);
            var userAduioStream = (InputStream)socketUser.AudioStream;

            string savePath = Path.Combine(DataPaths.VoiceRecordingPath, $"{ user.Id.ToString() }.wav");
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            using var ffmpeg = CreateFfmpegOut($"{ savePath }");
            using var ffmpegOutStdinStream = ffmpeg.StandardInput.BaseStream;
            DateTime start = DateTime.Now;
            try
            {
                _logger.LogInfo($"Listening to { user.Username }!");
                int recordSec = 10;
                var buffer = new byte[4096];
                while (await userAduioStream.ReadAsync(buffer, 0, buffer.Length) > 0 && (DateTime.Now - start).TotalSeconds <= recordSec)
                {
                    await ffmpegOutStdinStream.WriteAsync(buffer, 0, buffer.Length);
                    await ffmpegOutStdinStream.FlushAsync();
                }
            }
            finally
            {
                await ffmpegOutStdinStream.FlushAsync();
                ffmpegOutStdinStream.Close();
                // After its done KILL the process because when Close is used sometimes
                // the ffmpeg process does not close
                ffmpeg.Kill();
                _logger.LogInfo($"Finished listening to { user.Username }!");
            }

            try
            {
                string voiceCommandText = await _azureSpeech.RecognizeSpeechAsync(savePath);
                if (!string.IsNullOrWhiteSpace(voiceCommandText))
                {
                    VoiceCommand voiceCommand = voiceCommandText.ParseVoiceCommand();
                    if (voiceCommand.Invoke == "alexa")
                    {
                        switch (voiceCommand.Command)
                        {
                            case "play": await _audioService.DownloadAndPlayAudioAsync(voiceCommand.Argument, context); break;
                            case "leave": await _audioService.LeaveAudio(context.Guild); break;
                            default: await context.Channel.SendMessageAsync("Sorry, I didn't quite catch that."); break;
                        }
                    }
                }
            }
            finally
            {
                File.Delete(savePath);
            }
        }

        /// <summary>
        /// Create a ffmpeg process that will stream the audio data to a file
        /// </summary>
        /// <param name="savePath">The path to the file where the audio stream will be saved to</param>
        private Process CreateFfmpegOut(string savePath)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -ac 2 -f s16le -ar 48000 -i pipe:0 -ar 22050 \"{ savePath }\"",
                UseShellExecute = false,
                RedirectStandardInput = true,
            });
        }

        #endregion
    }
}
