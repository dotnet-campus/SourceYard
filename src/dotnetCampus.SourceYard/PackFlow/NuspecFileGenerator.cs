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
            var fileInfo = new FileInfo(context.ProjectFile);
            var doc = XDocument.Load(fileInfo.OpenRead());

            var element = doc.Root;

            if (element != null)
            {
                var id = context.PackageId;

                var title = context.ProjectName;

                var version = context.PackageVersion;

                //context.PackingFolder
                var csprojToNuspecFile = new CsprojToNuspecFile();

                var nuspecPackage = csprojToNuspecFile.Parse(element, id, title, version, context.BuildProps);
                return nuspecPackage;
            }

            return null;
        }
    }
}