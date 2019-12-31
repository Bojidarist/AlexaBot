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

        [Command("help")]
        public async Task HelpCommand()
        {
            await Context.Channel.SendMessageAsync("https://bojidarist.github.io/AlexaBot");
        }
    }
}
