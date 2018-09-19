using System.Xml.Serialization;

namespace dotnetCampus.SourceYard.PackFlow.Nuspec
{
    public class NuspecDependency
    {
        [XmlAttribute("id", Namespace = "")]
        public string Id { get; set; }

        [XmlAttribute(attributeName: "version")]
        public string Version { get; set; }
    }
}