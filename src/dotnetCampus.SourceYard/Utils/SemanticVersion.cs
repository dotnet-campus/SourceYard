using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace dotnetCampus.SourceYard.Utils
{
    /// <summary>
    ///     语义版本号
    /// </summary>
    public class SemanticVersion : IFormattable, IComparable<SemanticVersion>
    {
        /// <summary>
        /// 创建版本号
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="patch"></param>
        public SemanticVersion(int major = 0, int minor = 0, int patch = 0)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            PreReleaseTag = null;
            BuildMetaData = null;
        }

        /// <summary>
        /// 创建版本号
        /// </summary>
        /// <param name="semanticVersion"></param>
        public SemanticVersion(SemanticVersion semanticVersion)
        {
            Major = semanticVersion.Major;
            Minor = semanticVersion.Minor;
            Patch = semanticVersion.Patch;

            PreReleaseTag = semanticVersion.PreReleaseTag;
            BuildMetaData = semanticVersion.BuildMetaData;
        }

        /// <summary>
        /// 空白版本
        /// </summary>
        public static readonly SemanticVersion Empty = new SemanticVersion();

        private static readonly Regex ParseSemVer = new Regex(
            @"^(?<SemVer>(?<Major>\d+)(\.(?<Minor>\d+))(\.(?<Patch>\d+))?)(\.(?<FourthPart>\d+))?(-(?<Tag>[^\+]*))?(\+(?<BuildMetaData>.*))?$",
            RegexOptions.Compiled);

        /// <summary>
        /// 主版本号
        /// </summary>
        public int Major { set; get; }
        /// <summary>
        /// 次版本号
        /// </summary>
        public int Minor { set; get; }
        public int Patch { set; get; }
        public string? PreReleaseTag { set; get; }
        public string? BuildMetaData { set; get; }

        public bool Equals(SemanticVersion? obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return Major == obj.Major &&
                   Minor == obj.Minor &&
                   Patch == obj.Patch &&
                   PreReleaseTag == obj.PreReleaseTag &&
                   BuildMetaData == obj.BuildMetaData;
        }

        public bool IsEmpty()
        {
            return Equals(Empty);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((SemanticVersion) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Patch;
                hashCode = (hashCode * 397) ^ (PreReleaseTag != null ? PreReleaseTag.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BuildMetaData != null ? BuildMetaData.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(SemanticVersion v1, SemanticVersion v2)
        {
            if (ReferenceEquals(v1, null))
            {
                return ReferenceEquals(v2, null);
            }

            return v1.Equals(v2);
        }

        public static bool operator !=(SemanticVersion v1, SemanticVersion v2)
        {
            return !(v1 == v2);
        }

        public static bool operator >(SemanticVersion v1, SemanticVersion v2)
        {
            if (ReferenceEquals(v1, null))
            {
                throw new ArgumentNullException(nameof(v1));
            }

            if (ReferenceEquals(v2, null))
            {
                throw new ArgumentNullException(nameof(v2));
            }

            return v1.CompareTo(v2) > 0;
        }

        public static bool operator >=(SemanticVersion v1, SemanticVersion v2)
        {
            if (ReferenceEquals(v1, null))
            {
                throw new ArgumentNullException(nameof(v1));
            }

            if (ReferenceEquals(v2, null))
            {
                throw new ArgumentNullException(nameof(v2));
            }

            return v1.CompareTo(v2) >= 0;
        }

        public static bool operator <=(SemanticVersion v1, SemanticVersion v2)
        {
            if (ReferenceEquals(v1, null))
            {
                throw new ArgumentNullException(nameof(v1));
            }

            if (ReferenceEquals(v2, null))
            {
                throw new ArgumentNullException(nameof(v2));
            }

            return v1.CompareTo(v2) <= 0;
        }

        public static bool operator <(SemanticVersion? v1, SemanticVersion? v2)
        {
            if (ReferenceEquals(v1, null))
            {
                throw new ArgumentNullException(nameof(v1));
            }

            if (ReferenceEquals(v2, null))
            {
                throw new ArgumentNullException(nameof(v2));
            }

            return v1.CompareTo(v2) < 0;
        }

        public static SemanticVersion? Parse(string version, string tagPrefixRegex = "[Vv]")
        {
            if (!TryParse(version, tagPrefixRegex, out var semanticVersion))
            {
                throw new WarningException($"Failed to parse {version} into a Semantic Version");
            }

            return semanticVersion;
        }

        public static bool TryParse(string version, string tagPrefixRegex, out SemanticVersion? semanticVersion)
        {
            // 这一句为了替换 EasiNote 中包含占位符的版本号。
            version = version.Replace(".$", "-").Replace("$", "");

            // 以下是原始逻辑。
            var match = Regex.Match(version, $"^({tagPrefixRegex})?(?<version>.*)$");

            if (!match.Success)
            {
                semanticVersion = null;
                return false;
            }

            version = match.Groups["version"].Value;
            var parsed = ParseSemVer.Match(version);

            if (!parsed.Success)
            {
                semanticVersion = null;
                return false;
            }

            var semanticVersionBuildMetaData = parsed.Groups["BuildMetaData"].Value;
            var fourthPart = parsed.Groups["FourthPart"];

            semanticVersion = new SemanticVersion
            {
                Major = int.Parse(parsed.Groups["Major"].Value),
                Minor = parsed.Groups["Minor"].Success ? int.Parse(parsed.Groups["Minor"].Value) : 0,
                Patch = parsed.Groups["Patch"].Success ? int.Parse(parsed.Groups["Patch"].Value) : 0,
                PreReleaseTag = parsed.Groups["Tag"].Value,
                BuildMetaData = semanticVersionBuildMetaData
            };

            return true;
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public SemanticVersion IncrementVersion(VersionField incrementStrategy)
        {
            var incremented = new SemanticVersion(this);
            if (string.IsNullOrEmpty(incremented.PreReleaseTag))
            {
                switch (incrementStrategy)
                {
                    case VersionField.None:
                        break;
                    case VersionField.Major:
                        incremented.Major++;
                        incremented.Minor = 0;
                        incremented.Patch = 0;
                        break;
                    case VersionField.Minor:
                        incremented.Minor++;
                        incremented.Patch = 0;
                        break;
                    case VersionField.Patch:
                        incremented.Patch++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return incremented;
        }

        public static explicit operator Version(SemanticVersion version)
        {
            return new Version(version.Major, version.Minor, version.Patch, 0);
        }

        public int CompareTo(SemanticVersion? value)
        {
            if (ReferenceEquals(value, null))
            {
                return 1;
            }

            if (Major != value.Major)
            {
                if (Major > value.Major)
                {
                    return 1;
                }

                return -1;
            }

            if (Minor != value.Minor)
            {
                if (Minor > value.Minor)
                {
                    return 1;
                }

                return -1;
            }

            if (Patch != value.Patch)
            {
                if (Patch > value.Patch)
                {
                    return 1;
                }

                return -1;
            }

            if (PreReleaseTag != value.PreReleaseTag)
            {
                if (string.Compare(PreReleaseTag, value.PreReleaseTag, StringComparison.InvariantCulture) > 0)
                {
                    return 1;
                }

                return -1;
            }

            return 0;
        }

        /// <summary>
        ///     <para>s - Default SemVer [1.2.3-beta.4+5]</para>
        ///     <para>f - Full SemVer [1.2.3-beta.4+5]</para>
        ///     <para>i - Informational SemVer [1.2.3-beta.4+5.Branch.master.BranchType.Master.Sha.000000]</para>
        ///     <para>j - Just the SemVer part [1.2.3]</para>
        ///     <para>t - SemVer with the tag [1.2.3-beta.4]</para>
        ///     <para>l - Legacy SemVer tag for systems which do not support SemVer 2.0 properly [1.2.3-beta4]</para>
        ///     <para>lp - Legacy SemVer tag for systems which do not support SemVer 2.0 properly (padded) [1.2.3-beta0004]</para>
        /// </summary>
        public string ToString(string? format, IFormatProvider? formatProvider = null)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "s";
            }

            if (formatProvider?.GetFormat(GetType()) is ICustomFormatter formatter)
            {
                return formatter.Format(format, this, formatProvider);
            }

            // Check for lp first because the param can varry
            format = format!.ToLower();
            if (format.StartsWith("lp", StringComparison.Ordinal))
            {
                // handle the padding
                return !string.IsNullOrEmpty(PreReleaseTag) ? $"{ToString("j")}-{PreReleaseTag}" : ToString();
            }

            switch (format)
            {
                case "j":
                    return $"{Major}.{Minor}.{Patch}";
                case "s":
                    return !string.IsNullOrEmpty(PreReleaseTag)
                        ? $"{ToString("j")}-{PreReleaseTag}"
                        : ToString("j");
                case "t":
                    return !string.IsNullOrEmpty(PreReleaseTag)
                        ? $"{ToString("j")}-{PreReleaseTag}"
                        : ToString("j");
                case "l":
                    return !string.IsNullOrEmpty(PreReleaseTag)
                        ? $"{ToString("j")}-{PreReleaseTag}"
                        : ToString("j");
                case "f":
                {
                    var buildMetadata = BuildMetaData;

                    return !string.IsNullOrEmpty(buildMetadata)
                        ? $"{ToString("s")}+{buildMetadata}"
                        : ToString("s");
                }
                case "i":
                {
                    var buildMetadata = BuildMetaData;

                    return !string.IsNullOrEmpty(buildMetadata)
                        ? $"{ToString("s")}+{buildMetadata}"
                        : ToString("s");
                }
                default:
                    throw new ArgumentException($"Unrecognised format '{format}'", "format");
            }
        }
    }
}