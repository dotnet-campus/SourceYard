using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommandLine;
using dotnetCampus.SourceYard.Cli;
using dotnetCampus.SourceYard.Utils;

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
            if (options.IsDebug == "true")
            {
                Debugger.Launch();
                Console.WriteLine(Environment.CommandLine);
            }

            var logger = new Logger();

            try
            {
                logger.Message("开始打源码包");

                var projectFile = options.ProjectFile;
                var intermediateDirectory = options.IntermediateDirectory;
                var packageOutputPath = options.PackageOutputPath;
                var packageVersion = options.PackageVersion;

                logger.Message($@"项目文件 {projectFile}
临时文件{intermediateDirectory}
输出文件{packageOutputPath}
版本{packageVersion}
编译的文件{options.CompileFile}
资源文件{options.ResourceFile}
内容{options.ContentFile}
页面{options.Page}");

                var buildProps = new BuildProps()
                {
                    Authors = options.Authors,
                    Company = options.Company,
                    Owner = options.Owner ?? options.Authors,
                    Copyright = options.Copyright,
                    Description = options.Description,
                    PackageProjectUrl = options.PackageProjectUrl,
                    RepositoryType = options.RepositoryType,
                    RepositoryUrl = options.RepositoryUrl,
                    Title = options.Title,
                    PackageIconUrl = options.PackageIconUrl,
                    PackageLicenseUrl = options.PackageLicenseUrl,
                    PackageReleaseNotes = options.PackageReleaseNotes,
                    PackageTags = options.PackageTags
                };

                new Packer(projectFile: projectFile,
                    intermediateDirectory, packageOutputPath: packageOutputPath, 
                    packageVersion: packageVersion, 
                    compileFile: options.CompileFile,
                    resourceFile: options.ResourceFile, 
                    contentFile: options.ContentFile, 
                    page: options.Page, 
                    applicationDefinition: options.ApplicationDefinition, 
                    noneFile: options.None,
                    embeddedResource: options.EmbeddedResource,
                    buildProps: buildProps).Pack();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {
        }
    }
}
