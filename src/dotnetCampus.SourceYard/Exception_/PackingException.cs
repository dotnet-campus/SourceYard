using System;

namespace dotnetCampus.SourceYard
{
    internal class PackingException : Exception
    {
        public PackingException(string message,
            string? key = null,
            string? file = null) : base(message)
        {
            Key = key;
            File = file;
        }

        public string? Key { get; set; }

        public string? File { get; set; }
    }
}
