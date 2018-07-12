using System.IO;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.PackFlow
{
    internal class SourcePacker : IPackFlow
    {
        public void Pack(IPackingContext context)
        {
            var srcFolder = Path.Combine(context.PackingFolder, "src");
            FileSystem.CopyFolderContents(context.ProjectFolder, srcFolder, excludes: new[] { "bin", "obj" });
        }
    }
}
