using System.IO;
using System.Text;
using dotnetCampus.SourceYard.Context;

namespace dotnetCampus.SourceYard.PackFlow
{
    internal class EvaluatePacker : IPackFlow
    {
        public void Pack(IPackingContext context)
        {
            var copyFolder = Path.Combine(context.PackingFolder, "build");

            // 替换 props 和 targets 文件中的占位符。
            foreach (var file in new DirectoryInfo(copyFolder)
                .EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                var builder = new StringBuilder(File.ReadAllText(file.FullName, Encoding.UTF8));
                builder
                    .Replace("#(PackageId)", context.PackageId)
                    .Replace("#(PackageGuid)", context.PackageGuid);
                File.WriteAllText(file.FullName, builder.ToString(), Encoding.UTF8);
            }
        }
    }
}
