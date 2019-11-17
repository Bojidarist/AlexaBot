using AlexaBotConsoleApp.Data;
using AlexaBotConsoleApp.Loggers;
using System.IO;

namespace AlexaBotConsoleApp.Helpers
{
    public static class Initializer
    {
        /// <summary>
        /// Initializes everything in the <see cref="Initializer"/>
        /// </summary>
        public static void InitAll(ILogger logger)
        {
            InitPaths(logger);
        }

        /// <summary>
        /// Initializes all directories
        /// </summary>
        public static void InitPaths(ILogger logger)
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

            logger.LogInfo("Initialize paths");
        }
    }
}
