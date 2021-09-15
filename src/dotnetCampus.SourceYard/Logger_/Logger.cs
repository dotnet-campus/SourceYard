using dotnetCampus.MSBuildUtils;

namespace dotnetCampus.SourceYard
{
    internal class Logger : MSBuildConsoleLogger, ILogger
    {
        public void Warning(PackingException exception)
        {
            var message = exception.Message;
            if (!string.IsNullOrEmpty(exception.Key))
            {
                message = exception.Key + ": " + exception.Message;
            }

            Warning(message, targetFile:exception.File);
        }

        public void Error(PackingException exception)
        {
            var message = exception.Message;
            if (!string.IsNullOrEmpty(exception.Key))
            {
                message = exception.Key + ": " + exception.Message;
            }

            Error(message, targetFile: exception.File);
        }
    }
}
