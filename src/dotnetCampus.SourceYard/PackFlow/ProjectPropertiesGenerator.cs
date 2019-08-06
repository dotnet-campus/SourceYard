using System;
using System.IO;
using System.IO.Compression;
using dotnetCampus.SourceYard.Context;

namespace dotnetCampus.SourceYard.PackFlow
{
    /// <summary>
    /// 将源项目的一些属性存起来，这样可以在目标项目中使用。
    /// </summary>
    internal class ProjectPropertiesGenerator : IPackFlow
    {
        public void Pack(IPackingContext context)
        {
            // 生成源项目的属性。
        }
    }
}
