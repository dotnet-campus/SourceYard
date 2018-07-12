namespace dotnetCampus.SourceYard.Logger
{
    internal interface ILogger
    {
        void Message(string text);

        void Warning(string text);

        void Warning(PackingException exception);

        void Error(string text);

        void Error(PackingException exception);
    }
}
