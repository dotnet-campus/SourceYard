using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace dotnetCampus.SourceYard.PackFlow.Nuspec
{
    [Serializable]
    [XmlType(typeName: "metadata", Namespace = "")]
    public class NuspecMetadata
    {
        [XmlElement("description")]
        public string Description { set; get; }

        [XmlArray(elementName: "dependencies", Namespace = "")]
        [XmlArrayItem(elementName: "dependency")]
        public List<NuspecDependency> Dependencies { set; get; } = new List<NuspecDependency>();

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("copyright")]
        public string Copyright { get; set; }

        [XmlElement("licenseUrl")]
        public string PackageLicenseUrl { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("projectUrl")]
        public string PackageProjectUrl { get; set; }

        [XmlElement("iconUrl")]
        public string PackageIconUrl { get; set; }

        [XmlElement("tags")]
        public string PackageTags { get; set; }

        [XmlElement("releaseNotes")]
        public string PackageReleaseNotes { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("authors")]
        public string Authors { get; set; }

        [XmlElement("owners")]
        public string Owner { get; set; }
    }

    public class NugetTargetFramework
    {
        [XmlAttribute("targetFramework")]
        public string TargetFramework { get; set; } = ".NETFramework4.5";

        [XmlArray(elementName: "dependencies", Namespace = "")]
        [XmlArrayItem(elementName: "dependency")]
        public List<NuspecDependency> Dependencies { set; get; } = new List<NuspecDependency>();
    }
}