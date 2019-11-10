using Discord.Commands;
using System.Threading.Tasks;

namespace AlexaBotConsoleApp.Modules
{
    public class BasicTextCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingPongCommand()
        {
            await Context.Channel.SendMessageAsync("Pong");
        }
    }
}
