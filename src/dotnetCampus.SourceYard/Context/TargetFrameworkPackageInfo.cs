using System.IO;

namespace dotnetCampus.SourceYard.Context
{
    class TargetFrameworkPackageInfo
    {
        public TargetFrameworkPackageInfo(string targetFramework, DirectoryInfo sourcePackingFolder)
        {
            TargetFramework = targetFramework;
            SourcePackingFolder = sourcePackingFolder;
        }

        public bool IsValid
        {
            get { return _isValid ??= GetIsValid(); }
        }

        public string TargetFramework { get; }
        public DirectoryInfo SourcePackingFolder { get; }

        private bool GetIsValid()
        {
            // 判断此文件是否合法
            // 判断方法如获取 CompileFile.txt 是否存在
            var compileFile = Path.Combine(SourcePackingFolder.FullName, "CompileFile.txt");
            return File.Exists(compileFile);
        }

        private bool? _isValid;
    }
}