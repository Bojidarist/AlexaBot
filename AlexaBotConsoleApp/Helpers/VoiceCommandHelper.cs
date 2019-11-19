using AlexaBotConsoleApp.Models;
using System;
using System.Text;

namespace AlexaBotConsoleApp.Helpers
{
    public static class VoiceCommandHelper
    {
        /// <summary>
        /// Converts the string representation of a command to a <see cref="VoiceCommand"/>
        /// </summary>
        /// <param name="input">The string input</param>
        public static VoiceCommand ParseVoiceCommand(this string input)
        {
            char[] separators = new char[5] { ' ', '?', ',', '.', '!' };

            // Get the words in the input
            string[] command = input.ToLower().Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (command.Length < 2)
            {
                throw new ArgumentException("Incorrect command");
            }

            // Get the argument for the command
            StringBuilder commandArg = new StringBuilder();
            foreach (var cmd in command[2..])
            {
                commandArg.Append($"{ cmd } ");
            }

            return new VoiceCommand() { Invoke = command[0], Command = command[1], Argument = commandArg.ToString() };
        }
    }
}
