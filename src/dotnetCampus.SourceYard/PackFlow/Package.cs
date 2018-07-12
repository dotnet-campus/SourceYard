using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.PackFlow
{
    /// <summary>
    /// 引用的库
    /// </summary>
    public class Package
    {
        /// <inheritdoc />
        public Package(string packageName, ReferenceVersion referenceVersion)
        {
            PackageName = packageName;
            ReferenceVersion = referenceVersion;
        }

        /// <summary>
        /// 引用库
        /// </summary>
        public string PackageName { get; }

        /// <summary>
        /// 库的版本
        /// </summary>
        public ReferenceVersion ReferenceVersion { get; }
    }
}