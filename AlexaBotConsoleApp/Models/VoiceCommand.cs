namespace AlexaBotConsoleApp.Models
{
    public class VoiceCommand
    {
        /// <summary>
        /// The invoke string (alexa)
        /// </summary>
        public string Invoke { get; set; }

        /// <summary>
        /// The command string (play)
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// The argument string (despacito)
        /// </summary>
        public string Argument { get; set; }
    }
}
