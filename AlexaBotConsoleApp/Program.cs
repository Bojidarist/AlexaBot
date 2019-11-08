namespace AlexaBotConsoleApp
{
    class Program
    {
        static void Main()
        {
            // Start the bot
            new Bootstrap().StartAsync().GetAwaiter().GetResult();
        }
    }
}
