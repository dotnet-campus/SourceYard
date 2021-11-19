using System.IO;
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
}