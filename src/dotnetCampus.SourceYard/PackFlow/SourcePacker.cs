using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        
            FileSystem.CopyFileList(context.ProjectFolder, srcFolder, context.PackagedProjectFile.GetAllFile().ToList());
            context.Logger.Message("复制源代码文件完成");
        }
    }
}