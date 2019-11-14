using System.IO;

namespace AlexaBotConsoleApp.Data
{
    public static class DataPaths
    {
        /// <summary>
        /// The path to the audio resources
        /// </summary>
        public static string AudioPath { get; set; } = "Audio";

        /// <summary>
        /// The path to the downloaded songs by the bot
        /// </summary>
        public static string SongDownloadPath { get; set; } = Path.Combine(AudioPath, "Songs");

        /// <summary>
        /// The path to the recorded voices by the bot
        /// </summary>
        public static string VoiceRecordingPath { get; set; } = Path.Combine(AudioPath, "Voice");
    }
}
