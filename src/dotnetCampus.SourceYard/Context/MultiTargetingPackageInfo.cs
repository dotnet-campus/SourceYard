using System.Collections.Generic;
using System.IO;

namespace dotnetCampus.SourceYard.Context
{
    class MultiTargetingPackageInfo
    {
        public MultiTargetingPackageInfo(DirectoryInfo multiTargetingPackageInfoFolder)
        {
            MultiTargetingPackageInfoFolder = multiTargetingPackageInfoFolder;
            var folder = multiTargetingPackageInfoFolder.FullName;
            var targetFrameworkPackageInfoList = new List<TargetFrameworkPackageInfo>();

            foreach (var file in Directory.GetFiles(folder, "*.txt"))
            {
                var packageInfo = File.ReadAllText(file!);
                var sourcePackingFolder = packageInfo.Trim('\r', '\n');
                sourcePackingFolder = Path.GetFullPath(sourcePackingFolder);

                // 判断此文件是否合法
                // 判断方法如获取 CompileFile.txt 是否存在
                var compileFile = Path.Combine(sourcePackingFolder, "CompileFile.txt");
                if (File.Exists(compileFile))
                {
                    var targetFramework = Path.GetFileNameWithoutExtension(file);
                    targetFrameworkPackageInfoList.Add(new TargetFrameworkPackageInfo(targetFramework,
                        new DirectoryInfo(sourcePackingFolder)));
                }
            }

            TargetFrameworkPackageInfoList = targetFrameworkPackageInfoList;
        }

        public DirectoryInfo MultiTargetingPackageInfoFolder { get; }

        public IReadOnlyList<TargetFrameworkPackageInfo> TargetFrameworkPackageInfoList { get; }
    }
}