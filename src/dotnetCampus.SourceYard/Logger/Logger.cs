using System;
using System.Text;

namespace dotnetCampus.SourceYard.Logger
{
    internal class Logger : ILogger
    {
        public void Message(string text)
        {
            Console.WriteLine(text);
        }

        public void Warning(string text)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"warning: {text}");
            Console.ForegroundColor = color;
        }

        public void Warning(PackingException exception)
        {
            LogCore(exception, "warning");
        }

        public void Error(string text)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"error: {text}");
            Console.ForegroundColor = color;
        }

        public void Error(PackingException exception)
        {
            LogCore(exception, "error");
        }

        public void LogCore(PackingException exception, string type)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            var builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(exception.File))
            {
                builder.Append(exception.File);
            }

            builder.Append(type);

            if (!string.IsNullOrWhiteSpace(exception.Key))
            {
                builder.Append($" {exception.Key}");
            }

            builder.Append(": ");
            builder.Append(exception.Message);

            Console.ForegroundColor = color;
        }
    }
}
