using System.IO;

namespace dotnetCampus.SourceYard.Context
{
    /// <summary>
    /// 放在源代码包的文件
    /// </summary>
    public class SourceYardPackageFile
    {
        public SourceYardPackageFile(FileInfo sourceFile, string sourcePackagePath)
        {
            SourceFile = sourceFile;
            SourcePackagePath = sourcePackagePath;
        }

        /// <summary>
        /// 源代码的文件
        /// </summary>
        public FileInfo SourceFile { get; }

        /// <summary>
        /// 打包的路径
        /// </summary>
        public string SourcePackagePath { get; }
    }
}