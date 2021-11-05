using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace dotnetCampus.SourceYard.PackFlow.Nuspec
{
    [Serializable]
    [XmlType(typeName: "metadata", Namespace = "")]
    public class NuspecMetadata
    {
        public NuspecMetadata()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            SourceYardPackage = $"SourceYard {version}";
        }

        [XmlElement("description")] 
        public string? Description { set; get; }

        [XmlArray(elementName: "dependencies", Namespace = "")]
        [XmlArrayItem(elementName: "group")]
        public List<NuspecGroup> Dependencies { set; get; } = new List<NuspecGroup>();

        [XmlArray(elementName: "frameworkAssemblies", Namespace = "")]
        [XmlArrayItem(elementName: "frameworkAssembly")]
        public List<NuspecFrameworkAssembly> FrameworkAssemblies { set; get; } = new List<NuspecFrameworkAssembly>();

        [XmlElement("id")] 
        public string? Id { get; set; }

        [XmlElement("copyright")]
        public string? Copyright
        {
            get => _copyright;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    // 不使用空字符串，这样才能解决NuGet提示Copyright不能为空
                    value = null;
                }

                _copyright = value;
            }
        }

        [XmlElement("licenseUrl")]
        public string? PackageLicenseUrl
        {
            get => _packageLicenseUrl;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    // 不使用空字符串，这样才能解决NuGet提示licenseUrl不能为空
                    value = null;
                }

                _packageLicenseUrl = value;
            }
        }

        [XmlElement("version")]
        public string? Version { get; set; }

        [XmlElement("projectUrl")]
        public string? PackageProjectUrl
        {
            get => _packageProjectUrl;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    // 不使用空字符串，这样才能解决NuGet提示PackageProjectUrl不能为空
                    value = null;
                }

                _packageProjectUrl = value;
            }
        }

        [XmlElement("iconUrl")]
        public string? PackageIconUrl
        {
            get => _packageIconUrl;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    // 不使用空字符串，这样才能解决NuGet提示PackageProjectUrl不能为空
                    value = null;
                }

                _packageIconUrl = value;
            }
        }

        [XmlElement("tags")]
        public string? PackageTags { get; set; }

        [XmlElement("releaseNotes")]
        public string? PackageReleaseNotes { get; set; }

        [XmlElement("title")]
        public string? Title { get; set; }

        [XmlElement("authors")]
        public string? Authors { get; set; }

        [XmlElement("owners")]
        public string? Owner { get; set; }

        /// <summary>
        /// 表示 SourceYard 的包
        /// </summary>
        [XmlElement("SourceYardPackage")]
        public string? SourceYardPackage { set; get; }

        /// <summary>
        /// 通过这个属性可以在安装源代码包的时候默认选 private assets 这样就可以让安装源代码包的项目被引用的时候，引用的项目不需要再安装源代码包
        /// </summary>
        [XmlElement("developmentDependency")]
        public string? DevelopmentDependency { get; set; } = "true";

        [XmlElement("repository")]
        public Repository? Repository { set; get; }

        private string? _copyright;
        private string? _packageProjectUrl;
        private string? _packageLicenseUrl;
        private string? _packageIconUrl;
    }

    [XmlType(typeName: "repository", Namespace = "")]
    public class Repository
    {
        [XmlAttribute(attributeName: "type")]
        public string? Type { set; get; }

        [XmlAttribute(attributeName: "url")]
        public string? Url { set; get; }
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