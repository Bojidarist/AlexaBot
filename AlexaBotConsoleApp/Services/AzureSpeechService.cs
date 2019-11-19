using AlexaBotConsoleApp.Data;
using AlexaBotConsoleApp.Loggers;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Threading.Tasks;

namespace AlexaBotConsoleApp.Services
{
    public class AzureSpeechService
    {
        #region Private members

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
        public AzureSpeechService(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Recognize speech from a WAV audio file using azure cognitive services
        /// </summary>
        /// <param name="wavFile">The path to the WAV file</param>
        public async Task<string> RecognizeSpeechAsync(string wavFile)
        {
            var config = SpeechConfig.FromEndpoint(new Uri(SecretData.AzureSpeechEndpoint), SecretData.AzureSpeechToken);

            using var audioInput = AudioConfig.FromWavFileInput(wavFile);
            using var recognizer = new SpeechRecognizer(config, audioInput);
            _logger.LogInfo($"Recognizing: { wavFile }");
            var result = await recognizer.RecognizeOnceAsync();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                return result.Text;
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                throw new Exception($"NOMATCH: Speech could not be recognized.");
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                if (cancellation.Reason == CancellationReason.Error)
                {
                    _logger.LogError($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    _logger.LogError($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    _logger.LogError($"CANCELED: Did you update the subscription info?");
                }
                throw new Exception($"CANCELED: Reason={cancellation.Reason}");
            }

            return "";
        }

        #endregion
    }
}
