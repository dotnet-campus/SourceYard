using System;
using System.IO;
using System.IO.Compression;
using dotnetCampus.SourceYard.Context;

namespace dotnetCampus.SourceYard.PackFlow
{
    /// <summary>
    /// 将文件压缩为 nuget 包
    /// </summary>
    internal class NuGetPacker : IPackFlow
    {
        public void Pack(IPackingContext context)
        {
            var targetPackageFile = Path.GetFullPath(Path.Combine(context.PackageOutputPath,
                $"{context.PackageId}.{context.PackageVersion}.nupkg"));
            if (File.Exists(targetPackageFile))
            {
                context.Logger.Message($"发现{targetPackageFile}已经存在，将替换为新的文件");
                File.Delete(targetPackageFile);
            }

            var directory = Path.GetDirectoryName(targetPackageFile);
            if (directory == null)
            {
                throw new NotSupportedException("不支持相对路径。");
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // nuget 包就是一个 zip 压缩包
            ZipFile.CreateFromDirectory(context.PackingFolder, targetPackageFile);
        }
    }
}
