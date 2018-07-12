using System;
using System.Collections.Generic;
using CommandLine;
using dotnetCampus.SourceYard.Cli;

namespace dotnetCampus.SourceYard
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptionsAndReturnExitCode)
                .WithNotParsed(HandleParseError);
        }

        private static void RunOptionsAndReturnExitCode(Options options)
        {
            try
            {
                var projectFile = options.ProjectFile;
                var intermediateDirectory = options.IntermediateDirectory;
                var packageOutputPath = options.PackageOutputPath;
                var packageVersion = options.PackageVersion;
                new Packer(projectFile, intermediateDirectory, packageOutputPath, packageVersion).Pack();
            }
            catch (Exception e)
            {
                new Logger().Error(e.Message);
            }
        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {
        }
    }
}
