using System.Collections.Generic;
using System.IO;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.PackFlow
{
    internal class SourcePacker : IPackFlow
    {
        public void Pack(IPackingContext context)
        {
            context.Logger.Message("开始复制源代码文件");
            var srcFolder = Path.Combine(context.PackingFolder, "src");
            context.Logger.Message("源代码临时复制文件夹 " + srcFolder);
            FileSystem.IgnoreFileEndList = IgnoreFileEndList;
            FileSystem.IgnoreFolderList = IgnoreFolderList;
            FileSystem.CopyFolderContents(context.ProjectFolder, srcFolder, excludes: IgnoreFolderList);
            context.Logger.Message("复制源代码文件完成");
        }

        /// <summary>
        /// 忽略的文件夹列表
        /// </summary>
        private static IList<string> IgnoreFolderList { get; } = new List<string>()
        {
            ".vs", "bin", "obj", ".git", "x64", "x86"
        };

        /// <summary>
        /// 忽略的文件后缀列表
        /// </summary>
        private static IList<string> IgnoreFileEndList { get; } = new List<string>()
        {
            ".csproj.DotSettings", ".suo", ".user", ".sln.docstates", ".nupkg"
        };
    }
}