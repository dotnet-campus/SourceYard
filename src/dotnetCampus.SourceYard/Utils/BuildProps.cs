using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace dotnetCampus.SourceYard.Utils
{
    [XmlType("PropertyGroup")]
    public class BuildProps
    {
        public string Version { get; set; }

        public string PackageOutputPath { get; set; }

        public string Company { get; set; }

        public string Authors { get; set; }

        public string RepositoryUrl { get; set; }

        public string RepositoryType { get; set; }

        public string PackageProjectUrl { get; set; }

        public string Copyright { get; set; }

        public string Description { get; set; }

        public static BuildProps Parse(FileInfo file)
        {
            using (var stream = file.OpenText())
            {
                var load = XDocument.Load(stream);
                var propertyGroup = load.Root.DescendantsAndSelf(XName.Get("PropertyGroup")).FirstOrDefault();

                var xmlSerializer = new XmlSerializer(typeof(BuildProps));
                var buildProps = (BuildProps) xmlSerializer.Deserialize(propertyGroup.CreateReader());

                return buildProps;
            }
        }
    }
}