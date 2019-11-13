using AlexaBotConsoleApp.Services;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace AlexaBotConsoleApp.Modules
{
    public class AudioModule : ModuleBase<ICommandContext>
    {
        #region Private members

        // The AudioService used by the AudioModule
        private readonly AudioService _service;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="service">The <see cref="AudioService"/> used by the <see cref="AudioModule"/></param>
        public AudioModule(AudioService service)
        {
            _service = service;
        }

        #endregion

        #region Commands

        // You *MUST* mark these commands with 'RunMode.Async'
        // otherwise the bot will not respond until the Task times out.
        [Command("join", RunMode = RunMode.Async), Summary("Joins a voice channel")]
        public async Task JoinCommand()
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel, Context.Channel);
        }

        [Command("leave", RunMode = RunMode.Async), Summary("Leaves a voice channel")]
        public async Task LeaveCommand()
        {
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("play", RunMode = RunMode.Async), Summary("Plays audio in a voice channel")]
        public async Task PlayCommand([Remainder] string song = null)
        {
            await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
        }

        #endregion
    }
}
