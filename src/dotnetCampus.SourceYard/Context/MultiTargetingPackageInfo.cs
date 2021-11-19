﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using dotnetCampus.SourceYard.Cli;

namespace dotnetCampus.SourceYard.Context
{
    class MultiTargetingPackageInfo
    {
        public MultiTargetingPackageInfo(Options options)
            : this(new DirectoryInfo(options.MultiTargetingPackageInfoFolder), options.TargetFrameworks)
        {
        }

        public MultiTargetingPackageInfo(DirectoryInfo multiTargetingPackageInfoFolder, string? targetFrameworks)
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

            var validTargetFrameworkPackageInfoList = targetFrameworkPackageInfoList.ToList();
            validTargetFrameworkPackageInfoList.RemoveAll(t =>
            {
                if (t.IsValid is false)
                {
                    return true;
                }

                if (targetFrameworks != null && string.IsNullOrWhiteSpace(targetFrameworks) is false)
                {
                    // 对于单框架，也许 targetFrameworks 的值是空字符串
                    if (targetFrameworks.Contains(t.TargetFramework) is false)
                    {
                        return true;
                    }
                }

                return false;
            });

            ValidTargetFrameworkPackageInfoList = validTargetFrameworkPackageInfoList;
        }


        public DirectoryInfo MultiTargetingPackageInfoFolder { get; }

        public IReadOnlyList<TargetFrameworkPackageInfo> ValidTargetFrameworkPackageInfoList { get; }
        public IReadOnlyList<TargetFrameworkPackageInfo> TargetFrameworkPackageInfoList { get; }
    }
}