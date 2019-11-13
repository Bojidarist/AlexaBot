using AlexaBotConsoleApp.Services;
using Discord.Commands;
using System.Threading.Tasks;

namespace AlexaBotConsoleApp.Modules
{
    public class VoiceRecordingModule : ModuleBase<ICommandContext>
    {
        #region Private members

        // The VoiceRecordingService used by the VoiceRecordingModule
        private readonly VoiceRecordingService _service;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="service">The <see cref="VoiceRecordingService"/> used by the <see cref="VoiceRecordingModule"/></param>
        public VoiceRecordingModule(VoiceRecordingService service)
        {
            _service = service;
        }

        #endregion

        #region Commands

        [Command("listen", RunMode = RunMode.Async), Summary("Records a person's voice")]
        public async Task ListenCommand()
        {
            await _service.ListenUserAsync(await Context.Guild.GetUserAsync(Context.User.Id));
        }

        #endregion
    }
}
