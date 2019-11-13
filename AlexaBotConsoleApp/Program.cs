using AlexaBotConsoleApp.Loggers;

namespace AlexaBotConsoleApp
{
    class Program
    {
        static void Main()
        {
            // Start the bot
            new Bootstrap(new ConsoleLogger()).StartAsync().GetAwaiter().GetResult();
        }
    }
}
