using System;
using System.IO;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.PackFlow
{
    internal class AssetsPacker : IPackFlow
    {
        public void Pack(IPackingContext context)
        {
            // 将 Assets 文件夹中的所有文件复制到打包文件夹中。

            var assetsFolder = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location) ?? throw new InvalidOperationException(), "Assets", "Target",
                "build");

            var copyFolder = Path.Combine(context.PackingFolder, "build");
            FileSystem.CopyFolderContents(assetsFolder, copyFolder, name =>
                name.Replace("$(PackageId)", context.PackageId));
        }
    }
}
