using System;
using System.IO;
using System.Reflection;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.PackFlow
{
    internal class SelfPacker : IPackFlow
    {
        public void Pack(IPackingContext context)
        {
            var current = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            while (current != null && !current.Name.Equals("tools", StringComparison.OrdinalIgnoreCase))
            {
                current = current.Parent;
            }
            if (current is null)
            {
                return;
            }

            FileSystem.CopyFolderContents(current.FullName, Path.Combine(context.PackingFolder, "tools"));
        }
    }
}
