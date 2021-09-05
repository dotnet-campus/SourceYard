using dotnetCampus.MSBuildUtils;

namespace dotnetCampus.SourceYard
{
    internal interface ILogger: IMSBuildLogger
    {
        void Warning(PackingException exception);

        void Error(PackingException exception);
    }
}
