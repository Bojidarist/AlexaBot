using AlexaBotConsoleApp.Data;
using System.IO;

namespace AlexaBotConsoleApp.Helpers
{
    public static class Initializer
    {
        /// <summary>
        /// Initializes all directories
        /// </summary>
        public static void InitPaths()
        {
            if (Directory.Exists(DataPaths.AudioPath))
            {
                Directory.Delete(DataPaths.AudioPath, true);
            }

            if (Directory.Exists(DataPaths.SongDownloadPath))
            {
                Directory.Delete(DataPaths.SongDownloadPath, true);
            }
            Directory.CreateDirectory(DataPaths.SongDownloadPath);

            if (Directory.Exists(DataPaths.VoiceRecordingPath))
            {
                Directory.Delete(DataPaths.VoiceRecordingPath, true);
            }
            Directory.CreateDirectory(DataPaths.VoiceRecordingPath);

        }
    }
}
