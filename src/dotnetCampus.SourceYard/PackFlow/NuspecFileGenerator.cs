using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.PackFlow.nuspec;

namespace dotnetCampus.SourceYard.PackFlow
{
    /// <summary>
    /// 创建 Nuspec 文件
    /// </summary>
    internal class NuspecFileGenerator : IPackFlow
    {
        public void Pack(IPackingContext context)
        {
            var nuspecPackage = GetNuspec(context);

            var fileName = $"{context.PackageId}.nuspec";
            fileName = Path.Combine(context.PackingFolder, fileName);

            Write(nuspecPackage, fileName);
        }

        private void Write(NuspecPackage nuspecPackage, string fileName)
        {
            var file = new FileInfo(fileName);
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

#if DEBUG

            var str = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(str))
            {
                var xmlSerializer = new XmlSerializer(typeof(NuspecPackage));
                xmlSerializer.Serialize(xmlWriter, nuspecPackage, ns);
            }

            var rawceeyopereSuwhisa = str.ToString();

            rawceeyopereSuwhisa = rawceeyopereSuwhisa;
#endif

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
                    PackageReleaseNotes = buildProps.PackageReleaseNotes
                }
            };
        }
    }
}