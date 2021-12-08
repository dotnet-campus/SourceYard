using System.Xml.Serialization;

namespace dotnetCampus.SourceYard.PackFlow.NuspecFiles.NuspecContexts
{
    [XmlType("package")]
    public class NuspecPackage
    {
        [XmlElement("metadata")]
        public NuspecMetadata? NuspecMetadata { get; set; }
    }
}