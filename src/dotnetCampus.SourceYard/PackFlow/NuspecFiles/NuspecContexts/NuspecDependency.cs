using System.Xml.Serialization;

namespace dotnetCampus.SourceYard.PackFlow.NuspecFiles.NuspecContexts
{
    public class NuspecDependency
    {
        [XmlAttribute("id", Namespace = "")]
        public string Id { get; set; } = null!;

        [XmlAttribute(attributeName: "version")]
        public string Version { get; set; } = null!;
    }
}