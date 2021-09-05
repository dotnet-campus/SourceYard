using System.Text.RegularExpressions;

namespace dotnetCampus.SourceYard.Utils
{
    /// <summary>
    ///     引用的版本
    /// 用来转换  [2.1.0.293,3.0)、 (1.1.0.3,2.0]、 5.2 的版本
    /// </summary>
    public class ReferenceVersion
    {
        /// <summary>
        ///     创建引用版本
        /// </summary>
        /// <param name="version">版本</param>
        public ReferenceVersion(SemanticVersion? version)
        {
            Version = version;
            MinVersion = version;
            MaxVersion = version;
            IsIncludeMaxVersion = true;
            IsIncludeMinVersion = true;
        }

        /// <summary>
        ///     创建引用版本
        /// </summary>
        /// <param name="minVersion">最低版本</param>
        /// <param name="maxVersion">最高版本</param>
        /// <param name="isIncludeMinVersion">是否包括最低版本</param>
        /// <param name="isIncludeMaxVersion">是否包括最高版本</param>
        public ReferenceVersion(SemanticVersion? minVersion, SemanticVersion? maxVersion, bool isIncludeMinVersion,
            bool isIncludeMaxVersion)
        {
            Version = null;
            MinVersion = minVersion;
            MaxVersion = maxVersion;
            IsIncludeMinVersion = isIncludeMinVersion;
            IsIncludeMaxVersion = isIncludeMaxVersion;
        }

        /// <summary>
        ///     版本
        /// </summary>
        public SemanticVersion? Version { get; }

        /// <summary>
        ///     最低版本
        /// </summary>
        public SemanticVersion? MinVersion { get; }

        /// <summary>
        ///     最高版本
        /// </summary>
        public SemanticVersion? MaxVersion { get; }

        /// <summary>
        ///     是否包括最低版本
        /// </summary>
        public bool IsIncludeMinVersion { get; }

        /// <summary>
        ///     是否包括最高版本
        /// </summary>
        public bool IsIncludeMaxVersion { get; }

        /// <summary>
        ///     转换版本
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ReferenceVersion Parser(string str)
        {
            if (_regex == null)
            {
                _regex = new Regex(@"([(|\[])([\d|.]*),([\d|.]*)([)|\]])", RegexOptions.Compiled);
            }

            var res = _regex.Match(str);

            if (res.Success)
            {
                var isIncludeMinVersion = res.Groups[1].Value;
                var minVersion = res.Groups[2].Value;
                var maxVersion = res.Groups[3].Value;
                var isIncludeMaxVersion = res.Groups[4].Value;

                return new ReferenceVersion
                (
                    string.IsNullOrEmpty(minVersion) ? null : SemanticVersion.Parse(minVersion),
                    string.IsNullOrEmpty(maxVersion) ? null : SemanticVersion.Parse(maxVersion),
                    isIncludeMinVersion.Equals("["),
                    isIncludeMaxVersion.Equals("]")
                );
            }

            return new ReferenceVersion(SemanticVersion.Parse(str));
        }

        private static Regex? _regex;
    }
}