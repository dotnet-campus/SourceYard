using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.PackFlow.Nuspec;

namespace dotnetCampus.SourceYard.PackFlow
{
    /// <summary>
    /// 创建 Nuspec 文件
    /// </summary>
    internal class NuspecFileGenerator : IPackFlow
    {
        public void Pack(IPackingContext context)
        {
            _log = context.Logger;
            _log.Message("开始创建 nuspec 文件");
            var nuspecPackage = GetNuspec(context);

            var fileName = $"{context.PackageId}.nuspec";
            fileName = Path.Combine(context.PackingFolder, fileName);

            Write(nuspecPackage, fileName);
            _log.Message("完成创建 nuspec 文件");
        }

        private ILogger _log = null!;

        private void Write(NuspecPackage nuspecPackage, string fileName)
        {
            var file = new FileInfo(fileName);
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            //#if DEBUG

            //            var str = new StringBuilder();
            //            using (var xmlWriter = XmlWriter.Create(str))
            //            {
            //                var xmlSerializer = new XmlSerializer(typeof(NuspecPackage));
            //                xmlSerializer.Serialize(xmlWriter, nuspecPackage, ns);
            //            }

            //            var rawceeyopereSuwhisa = str.ToString();

            //            rawceeyopereSuwhisa = rawceeyopereSuwhisa;
            //#endif

            using (var stream = file.OpenWrite())
            {
                var xmlSerializer = new XmlSerializer(typeof(NuspecPackage));
                xmlSerializer.Serialize(stream, nuspecPackage, ns);
            }
        }

        private NuspecPackage GetNuspec(IPackingContext context)
        {
            var buildProps = context.BuildProps;

            Repository? repository = null;
            if (!string.IsNullOrEmpty(buildProps.RepositoryType) && !string.IsNullOrEmpty(buildProps.RepositoryUrl))
            {
                repository = new Repository()
                {
                    Type = buildProps.RepositoryType,
                    Url = buildProps.RepositoryUrl
                };
            }

            return new NuspecPackage()
            {
                NuspecMetadata = new NuspecMetadata()
                {
                    Authors = buildProps.Authors,
                    Copyright = buildProps.Copyright,
                    Description = buildProps.Description,
                    PackageProjectUrl = buildProps.PackageProjectUrl,
                    Version = context.PackageVersion,
                    Id = context.PackageId,
                    Owner = buildProps.Owner,
                    Title = buildProps.Title,
                    PackageIconUrl = buildProps.PackageIconUrl,
                    PackageLicenseUrl = buildProps.PackageLicenseUrl,
                    PackageTags = buildProps.PackageTags,
                    PackageReleaseNotes = buildProps.PackageReleaseNotes,
                    Dependencies = DependenciesParser.GetDependencies(context, _log),
                    Repository = repository
                }
            };
        }
    }

    static class DependenciesParser
    {
        /// <summary>
        /// 获取依赖内容
        /// </summary>
        /// <returns></returns>
        public static List<NuspecGroup> GetDependencies(IPackingContext context, ILogger logger)
        {
            _log = logger;
            var nuspecGroups = new List<NuspecGroup>();

            foreach (var targetFrameworkPackageInfo in context.MultiTargetingPackageInfo.TargetFrameworkPackageInfoList)
            {
                var sourcePackingFolder = targetFrameworkPackageInfo.SourcePackingFolder.FullName;

                var packageReferenceVersionFile = Path.Combine(sourcePackingFolder, "PackageReferenceVersionFile.txt");

                var dependencies = GetDependencies(packageReferenceVersionFile, context.BuildProps.SourceYardPackageReferenceList,
                    context.BuildProps.SourceYardExcludePackageReferenceList);

                nuspecGroups.Add(new NuspecGroup()
                {
                    TargetFramework = targetFrameworkPackageInfo.TargetFramework,
                    Dependencies = dependencies
                });
            }

            return nuspecGroups;
        }

        private static ILogger _log;

        /// <summary>
        /// 获取依赖
        /// </summary>
        /// <param name="contextPackageVersion">引用的NuGet包于版本号</param>
        /// <param name="sourceYardPackageReferenceList">源代码包</param>
        /// <param name="excludePackageReferenceList">排除的依赖</param>
        /// <returns></returns>
        private static List<NuspecDependency> GetDependencies(string contextPackageVersion,
            List<string> sourceYardPackageReferenceList, List<string> excludePackageReferenceList)
        {
            var nuspecDependencyList = ParserPackageVersion(contextPackageVersion, sourceYardPackageReferenceList);

            // 如果在排除列表就移除
            if (excludePackageReferenceList.Count > 0)
            {
                var excludeList = new HashSet<string>(excludePackageReferenceList);
                nuspecDependencyList.RemoveAll(temp => excludeList.Contains(temp.Id));
            }

            return nuspecDependencyList;
        }

        private static List<NuspecDependency> ParserPackageVersion(string packageVersionFile,
            List<string> sourceYardPackageReferenceList)
        {
            var nuspecDependencyList = new List<NuspecDependency>();
            var packageVersionRegex = new Regex(@"Name='(\S+)' Version='([\S|\-]+)' PrivateAssets='(\S*)'");
            using (var stream = File.OpenText(packageVersionFile))
            {
                string? line;
                while ((line = stream.ReadLine()) != null)
                {
                    var match = packageVersionRegex.Match(line);
                    if (match.Success)
                    {
                        var name = match.Groups[1].Value;
                        var version = match.Groups[2].Value;
                        var privateAssets = match.Groups[3].Value;

                        // 在源代码包如果项目引用的是 private asset 的库，那么就不应该添加引用
                        // 因为源代码是没有框架的依赖，对 sdk 带的库也不添加

                        var isPrivateAssetsAll = privateAssets.IndexOf("all", StringComparison.OrdinalIgnoreCase) >= 0;
                        // net45 没有下面方法
                        //var isPrivateAssetsAll = privateAssets.Contains("all", comparer);

                        // 解决 https://github.com/dotnet-campus/SourceYard/issues/60
                        // 即使有某个包标记了使用 private asset 是 All 的，但是这个包是一个源代码包，那么打包的时候就需要添加他的引用依赖
                        var includeInSourceYardPackageReference =
                            sourceYardPackageReferenceList.Any(temp =>
                                temp.Equals(name, StringComparison.OrdinalIgnoreCase));

                        if ((!isPrivateAssetsAll || includeInSourceYardPackageReference)
                            && !SDKNuget.Contains(name))
                        {
                            nuspecDependencyList.Add(new NuspecDependency()
                            {
                                Id = name,
                                Version = version
                            });
                        }
                    }
                    else
                    {
                        _log.Warning($"项目所引用的 NuGet 包包含有无法识别的格式，包信息：{line}");
                    }
                }
            }

            return nuspecDependencyList;
        }

        /// <summary>
        /// 包含在 sdk 的库，这些库不应该加入引用
        /// </summary>
        private static string[] SDKNuget { get; } = new[]
        {
            "Microsoft.NETCore.App",
            "Microsoft.NETCore.Platforms"
        };
    }
}