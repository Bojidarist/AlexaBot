using Discord;
using Discord.Audio.Streams;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AlexaBotConsoleApp.Services
{
    public class VoiceRecordingService
    {
        #region Methods

        /// <summary>
        /// Listens to a user and records the audio
        /// </summary>
        /// <param name="user">The user</param>
        public async Task ListenUserAsync(IGuildUser user)
        {
            var socketUser = (user as SocketGuildUser);
            var userAduioStream = (InputStream)socketUser.AudioStream;

            using var ffmpeg = CreateFfmpegOut("out.wav");
            using var ffmpegOutStdinStream = ffmpeg.StandardInput.BaseStream;
            DateTime start = DateTime.Now;
            try
            {
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
                Arguments = $"-hide_banner -ac 2 -f s16le -ar 48000 -i pipe:0 -ar 22050 \"{savePath}\"",
                UseShellExecute = false,
                RedirectStandardInput = true,
            });
        }

        #endregion
    }
}
