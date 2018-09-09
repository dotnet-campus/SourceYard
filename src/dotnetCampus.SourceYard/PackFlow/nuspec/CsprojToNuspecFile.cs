using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.PackFlow.nuspec
{
    public class CsprojToNuspecFile
    {
        public NuspecPackage Parse(XElement element, string id, string title, string version, BuildProps buildProps)
        {
            if (buildProps == null)
            {
                buildProps = new BuildProps();
            }

            var authors = GetAuthors(element); //??"lindexi";//找不到就用我名字
            var owner = GetOwners(element) ?? authors;
            if (string.IsNullOrEmpty(authors))
            {
                authors = owner;
            }

            authors = authors ?? buildProps.Authors ?? buildProps.Company;
            owner = owner ?? authors;

            var description = GetDescription(element) ?? buildProps.Description;
            var copyright = GetCopyright(element) ?? buildProps.Copyright;
            var packageLicenseUrl = GetPackageLicenseUrl(element) ?? buildProps.PackageProjectUrl;

            var packageList = GetPackageReference(element);
            packageList.AddRange(GetProjectReference(element, version));

            var packageProjectUrl = GetPackageProjectUrl(element) ?? buildProps.PackageProjectUrl;
            var packageIconUrl = GetPackageIconUrl(element);
            var packageTags = GetPackageTags(element);
            var packageReleaseNotes = GetPackageReleaseNotes(element);

            var nuspecMetadata = new NuspecMetadata
            {
                Id = id,
                Copyright = copyright,
                PackageLicenseUrl = packageLicenseUrl,
                Version = version,
                Dependencies = GetDependencies(packageList),
                PackageProjectUrl = packageProjectUrl,
                PackageIconUrl = packageIconUrl,
                PackageTags = packageTags,
                PackageReleaseNotes = packageReleaseNotes,
                Title = title,
                Authors = authors,
                Owner = owner,
                Description = description,
            };

            var nuspecPackage = new NuspecPackage()
            {
                NuspecMetadata = nuspecMetadata
            };

            return nuspecPackage;
        }

        private IEnumerable<Package> GetProjectReference(XElement element, string version)
        {
            return Enumerable.Empty<Package>();
            var packageList = new List<Package>();
            var regex = new Regex(@"([\w|\.]+)\.csproj");
            var referenceVersion = ReferenceVersion.Parser(version);

            foreach (var temp in element.DescendantsAndSelf(XName.Get("ProjectReference")))
            {
                var package = temp.Attribute(XName.Get("Include"))?.Value;
                if (!string.IsNullOrEmpty(package))
                {
                    package = regex.Match(package).Groups[1].Value;
                }

                packageList.Add(new Package(package, referenceVersion));
            }

            return packageList;
        }

        private List<NuspecDependency> GetDependencies(IEnumerable<Package> packageList)
        {
            //return new List<NugetTargetFramework>()
            //{
            //    new NugetTargetFramework()
            //    {
            //        Dependencies = packageList.Select(temp => new NuspecDependency
            //        {
            //            Id = temp.ProjectName,
            //            Version = GetPackageVersion(temp.ReferenceVersion)
            //        }).ToList()
            //    }
            //};

            return packageList.Select(temp => new NuspecDependency
            {
                Id = temp.PackageName,
                Version = GetPackageVersion(temp.ReferenceVersion),
            }).ToList();
        }

        private string GetPackageVersion(ReferenceVersion version)
        {
            if
            (
                version.Version != null
                && version.MaxVersion == version.Version
                && version.MinVersion == version.Version
            )
            {
                return version.Version.ToString();
            }

            var str = new StringBuilder();

            if (version.IsIncludeMinVersion)
            {
                str.Append("[");
            }
            else
            {
                str.Append("(");
            }

            if (version.MinVersion != null)
            {
                str.Append(version.MinVersion.ToString());
            }

            str.Append(",");

            if (version.MaxVersion != null)
            {
                str.Append(version.MaxVersion.ToString());
            }

            str.Append
            (
                version.IsIncludeMaxVersion ? "]" : ")"
            );

            return str.ToString();
        }

        private string GetPackageReleaseNotes(XElement element)
        {
            return GetString(element, "PackageReleaseNotes");
        }

        private string GetPackageTags(XElement element)
        {
            return GetString(element, "PackageTags");
        }

        private string GetPackageIconUrl(XElement element)
        {
            //此 NuGet 包的图标 url，无论是 nuget.org 还是 Visual Studio 都将从这个 url 下载包的图标。
            return GetString(element, "PackageIconUrl");
        }

        private string GetPackageProjectUrl(XElement element)
        {
            //此 NuGet 包的项目 url
            return GetString(element, "PackageProjectUrl");
        }

        private string GetPackageLicenseUrl(XElement element)
        {
            // 此 NuGet 包协议所在的 url
            return GetString(element, "PackageLicenseUrl");
        }

        private string GetCopyright(XElement element)
        {
            return GetString(element, "Copyright");
        }

        private string GetDescription(XElement element)
        {
            return GetString(element, "Description") ?? GetString(element, "PackageDescription");
        }

        private string GetOwners(XElement element)
        {
            return GetString(element, "Owner") ?? GetString(element, "Company");
        }

        private string GetAuthors(XElement element)
        {
            return GetString(element, "Authors");
        }

        private string GetString(XElement element, string str)
        {
            return element.DescendantsAndSelf(XName.Get(str)).FirstOrDefault()?.Value;
        }

        private static List<Package> GetPackageReference(XElement element)
        {
            var packageList = new List<Package>();
            var packageReferenceList = new List<string>();
            foreach (var temp in element.DescendantsAndSelf(XName.Get("PackageReference")))
            {
                var package = temp.Attribute(XName.Get("Include"));
                var version = temp.Attribute(XName.Get("Version"));

                if (package == null || version == null)
                {
                    continue;
                }

                packageList.Add(new Package(package.Value, ReferenceVersion.Parser(version.Value)));

                packageReferenceList.Add(temp.ToString());
            }


            return packageList;
        }
    }
}