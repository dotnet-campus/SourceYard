using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace dotnetCampus.SourceYard.Utils
{
    public class BuildProps
    {
        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Authors { get; set; }

        /// <summary>
        /// 仓库地址
        /// </summary>
        public string RepositoryUrl { get; set; }

        public string RepositoryType { get; set; }

        /// <summary>
        /// 项目地址
        /// </summary>
        public string PackageProjectUrl { get; set; }

        /// <summary>
        /// 版权
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public string Owner { get; set; }

        public string Title { get; set; }

        public string PackageLicenseUrl { get; set; }

        public string PackageIconUrl { get; set; }

        public string PackageReleaseNotes { get; set; }

        public string PackageTags { get; set; }
    }
}