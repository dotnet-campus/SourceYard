using System.Xml.Serialization;

namespace dotnetCampus.SourceYard.PackFlow.NuspecFiles.NuspecContexts
{
    public class NuspecFrameworkAssembly
    {
        [XmlAttribute("assemblyName", Namespace = "")]
        public string AssemblyName { get; set; } = null!;
        [XmlAttribute("targetFramework")]
        public string TargetFramework { get; set; } = null!;
    }
}