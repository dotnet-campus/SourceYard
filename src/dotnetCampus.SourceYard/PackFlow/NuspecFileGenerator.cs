using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
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

        private ILogger _log;

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
                    Dependencies = GetDependencies(context.PackageReferenceVersion)
                }
            };
        }

        private List<NuspecDependency> GetDependencies(string contextPackageVersion)
        {
            return ParserPackageVersion(contextPackageVersion);
        }

        private List<NuspecDependency> ParserPackageVersion(string packageVersionFile)
        {
            var nuspecDependencyList = new List<NuspecDependency>();
            var packageVersionRegex = new Regex(@"Name='(\S+)' Version='([\S|\-]+)' PrivateAssets='(\S*)'");
            using (var stream = File.OpenText(packageVersionFile))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    var match = packageVersionRegex.Match(line);
                    if (match.Success)
                    {
                        var name = match.Groups[1].Value;
                        var version = match.Groups[2].Value;
                        var privateAssets = match.Groups[3].Value;

                        if (!privateAssets.Contains("all")
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
        private string[] SDKNuget { get; } = new[]
        {
            "Microsoft.NETCore.App"
        };
    }
}