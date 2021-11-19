using System.Xml.Serialization;

namespace dotnetCampus.SourceYard.PackFlow.Nuspec
{
    public class NuspecFrameworkAssembly
    {
        [XmlAttribute("assemblyName", Namespace = "")]
        public string AssemblyName { get; set; } = null!;
        [XmlAttribute("targetFramework")]
        public string TargetFramework { get; set; } = null!;
    }
}