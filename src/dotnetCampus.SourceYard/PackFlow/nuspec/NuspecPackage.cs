using System.Xml.Serialization;

namespace dotnetCampus.SourceYard.PackFlow.Nuspec
{
    [XmlType("package")]
    public class NuspecPackage
    {
        [XmlElement("metadata")]
        public NuspecMetadata NuspecMetadata { get; set; }
    }
}