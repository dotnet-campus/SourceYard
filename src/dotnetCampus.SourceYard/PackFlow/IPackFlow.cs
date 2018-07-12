using dotnetCampus.SourceYard.Context;

namespace dotnetCampus.SourceYard.PackFlow
{
    internal interface IPackFlow
    {
        void Pack(IPackingContext context);
    }
}
