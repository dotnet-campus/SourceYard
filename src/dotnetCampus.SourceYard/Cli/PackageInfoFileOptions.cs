using dotnetCampus.Cli;

namespace dotnetCampus.SourceYard.Cli
{
    [Verb("Write")]
    class PackageInfoFileOptions
    {
        [Option("MultiTargetingPackageInfoFile")]
        public string MultiTargetingPackageInfoFile { set; get; } = null!;

        [Option(longName: "TargetFramework")]
        public string? TargetFramework { get; set; }
    }
}