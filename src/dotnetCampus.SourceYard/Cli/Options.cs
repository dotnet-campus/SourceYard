using System.Collections.Generic;
using CommandLine;

namespace dotnetCampus.SourceYard.Cli
{
    internal class Options
    {
        [Option('p', "project", Required = true, HelpText = "The full path of the project file.")]
        public string ProjectFile { get; set; }

        [Option('i', "intermediate-directory", Required = true, HelpText = "The relative path of the project intermediate output path, commonly the 'obj' folder and value in 'obj'.")]
        public string IntermediateDirectory { get; set; }

        [Option('n', "package-output-path", Required = true, HelpText = "The package output full path of the project.")]
        public string PackageOutputPath { get; set; }

        [Option('v', "package-version", Required = true, HelpText = "The package version value.")]
        public string PackageVersion { get; set; }
    }
}
