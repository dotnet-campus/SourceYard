using System.Collections.Generic;
using System.Xml.Serialization;

namespace dotnetCampus.SourceYard.PackFlow.NuspecFiles.NuspecContexts
{
    public class NuspecGroup
    {
        [XmlAttribute("targetFramework", Namespace = "")]
        public string TargetFramework { get; set; } = null!;
        [XmlElement(elementName: "dependency")]
        public List<NuspecDependency> Dependencies { set; get; } = new List<NuspecDependency>();
    }
}