using System.Collections.Generic;
using CommandLine;

namespace dotnetCampus.SourceYard.Cli
{
    internal class Options
    {
        [Option(longName: "debug")]
        public string IsDebug { get; set; }

        [Option('p', "project", Required = true, HelpText = "The full path of the project file.")]
        public string ProjectFile { get; set; }

        [Option('i', "intermediate-directory", Required = true, HelpText = "The relative path of the project intermediate output path, commonly the 'obj' folder and value in 'obj'.")]
        public string IntermediateDirectory { get; set; }

        [Option('n', "package-output-path", Required = true, HelpText = "The package output full path of the project.")]
        public string PackageOutputPath { get; set; }

        [Option('v', "package-version", Required = true, HelpText = "The package version value.")]
        public string PackageVersion { get; set; }

        [Option(longName: "Compile", HelpText = "编译的文件")]
        public string CompileFile { get; set; }

        [Option(longName: "Resource", HelpText = "资源文件")]
        public string ResourceFile { get; set; }

        [Option(longName: "Content")]
        public string ContentFile { get; set; }

        [Option(longName: "Page")]
        public string Page { get; set; }

        [Option(longName: "Company")]

        public string Company { get; set; }

        [Option(longName: "Authors")]
        public string Authors { get; set; }

        [Option(longName: "RepositoryUrl")]
        public string RepositoryUrl { get; set; }

        [Option(longName: "RepositoryType")]
        public string RepositoryType { get; set; }

        [Option(longName: "PackageProjectUrl")]
        public string PackageProjectUrl { get; set; }

        [Option(longName: "Copyright")]
        public string Copyright { get; set; }

        [Option(longName: "Description")]
        public string Description { get; set; }

        [Option(longName: "ApplicationDefinition")]
        public string ApplicationDefinition { get; set; }

        [Option(longName: "Owner")]
        public string Owner { get; set; }

        [Option(longName: "Title")]
        public string Title { get; set; }

        [Option(longName: "PackageLicenseUrl")]
        public string PackageLicenseUrl { get; set; }

        [Option(longName: "PackageIconUrl")]
        public string PackageIconUrl { get; set; }

        [Option(longName: "PackageReleaseNotes")]
        public string PackageReleaseNotes { get; set; }

        [Option(longName: "PackageTags")]
        public string PackageTags { get; set; }
    }
}
