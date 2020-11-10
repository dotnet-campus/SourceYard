using System.IO;

namespace dotnetCampus.SourceYard.Context
{
    /// <summary>
    /// 放在源代码包的文件
    /// </summary>
    public class SourceYardPackageFile
    {
        /// <summary>
        /// 创建放在源代码包的文件
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="sourcePackagePath"></param>
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