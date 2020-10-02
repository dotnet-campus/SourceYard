using System;
using dotnetCampus.Cli;
using dotnetCampus.Cli.Standard;

namespace dotnetCampus.SourceYard.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parse(args).AddStandardHandlers().AddHandler<InstallSourceOption>(InstallSource).Run();
        }

        private static void InstallSource(InstallSourceOption option)
        {
           // 下载对应的库
        }
    }

    class InstallSourceOption
    {
        [Option(longName: nameof(Package))] 
        public string Package { get; set; } = null!;

        [Option(longName: nameof(Version))]
        public string? Version { get; set; }
    }
}
