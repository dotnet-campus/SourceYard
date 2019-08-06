using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
#if DEBUG
            Debugger.Launch();
            Console.WriteLine(Environment.CommandLine);
#endif
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

                var description = ReadFile(options.DescriptionFile);
                var copyright = ReadFile(options.CopyrightFile);

                var buildProps = new BuildProps()
                {
                    Authors = options.Authors,
                    Company = options.Company,
                    Owner = options.Owner ?? options.Authors,
                    Copyright = copyright,
                    Description = description,
                    PackageProjectUrl = options.PackageProjectUrl,
                    RepositoryType = options.RepositoryType,
                    RepositoryUrl = options.RepositoryUrl,
                    Title = options.Title,
                    PackageIconUrl = options.PackageIconUrl,
                    PackageLicenseUrl = options.PackageLicenseUrl,
                    PackageReleaseNotes = options.PackageReleaseNotesFile,
                    PackageTags = options.PackageTags
                };

                new Packer(projectFile: projectFile,
                    intermediateDirectory: intermediateDirectory,
                    packageOutputPath: packageOutputPath,
                    packageVersion: packageVersion,
                    compileFile: options.CompileFile,
                    resourceFile: options.ResourceFile,
                    contentFile: options.ContentFile,
                    page: options.Page,
                    applicationDefinition: options.ApplicationDefinition,
                    noneFile: options.None,
                    embeddedResource: options.EmbeddedResource,
                    packageId: options.PackageId,
                    buildProps: buildProps,
                    packageReferenceVersion: options.PackageReferenceVersion,
                    rootNamespace: options.RootNamespace).Pack();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            string ReadFile(string file)
            {
                if (string.IsNullOrEmpty(file))
                {
                    return "";
                }

                file = Path.GetFullPath(file);
                if (File.Exists(file))
                {
                    try
                    {
                        return File.ReadAllText(file);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                    }
                }

                return "";
            }
        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {
            var logger = new Logger();
            foreach (var temp in errors)
            {
                logger.Error(temp.ToString());
            }
        }
    }
}
