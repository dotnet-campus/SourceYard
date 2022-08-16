using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using dotnetCampus.Cli;
using dotnetCampus.Cli.Standard;
using dotnetCampus.SourceYard.Cli;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MagicTransformMultiTargetingToFirstTarget(args);

            CommandLine.Parse(args).AddStandardHandlers()
                .AddHandler<Options>(RunOptionsAndReturnExitCode)
                .Run();
        }

        /// <summary>
        /// 更改命令行传入内容，用于方便接入后续步骤
        /// </summary>
        /// <param name="args"></param>
        private static void MagicTransformMultiTargetingToFirstTarget(string[] args)
        {
            var argDict = dotnetCampus.Cli.CommandLine.Parse(args).ToDictionary(
                            x => x.Key,
                            x => x.Value?.FirstOrDefault()?.Trim());
            var targetFrameworks = argDict["TargetFrameworks"];
            if (targetFrameworks != null && !string.IsNullOrWhiteSpace(targetFrameworks))
            {
                // 这里必须考虑如下情况
                /*
                 * <TargetFrameworks>netcoreapp3.1;net45;net6.0</TargetFrameworks>
                 *
                 * <PackageReference Include="dotnetCampus.SourceYard" Version="0.1.19393-alpha10" Condition="'$(TargetFramework)'=='net45'">
                       <PrivateAssets>all</PrivateAssets>
                       <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
                   </PackageReference>
                 */
                // 为什么只有 net45 才安装？原因是只有单个框架打包才有此需求
                // 此时就不合适获取首个，而是应该获取有带打包的

                string? firstTargetFramework = null;

                var multiTargetingPackageInfoFolder = argDict["MultiTargetingPackageInfoFolder"];
                if (multiTargetingPackageInfoFolder != null && !string.IsNullOrEmpty(multiTargetingPackageInfoFolder) &&
                    Directory.Exists(multiTargetingPackageInfoFolder))
                {
                    var multiTargetingPackageInfo =
                        new MultiTargetingPackageInfo(new DirectoryInfo(multiTargetingPackageInfoFolder),
                            targetFrameworks);
                    firstTargetFramework = multiTargetingPackageInfo.ValidTargetFrameworkPackageInfoList.FirstOrDefault()?.TargetFramework;
                }
                if(string.IsNullOrEmpty(firstTargetFramework))
                {
                    firstTargetFramework = targetFrameworks.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                }

                if (!string.IsNullOrWhiteSpace(firstTargetFramework))
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        args[i] = args[i].Replace("\\SourcePacking\\", $"\\{firstTargetFramework}\\SourcePacking\\");
                    }
                }
            }
        }

        private static void RunOptionsAndReturnExitCode(Options options)
        {
#if DEBUG
            Debugger.Launch();
            Console.WriteLine(Environment.CommandLine);
#endif
            var logger = new Logger();

            FixOptions(options);

            try
            {
                logger.Message("Source packaging");

                var projectFile = options.ProjectFile;
                var multiTargetingPackageInfo = new MultiTargetingPackageInfo(options);
                var intermediateDirectory = GetIntermediateDirectory(multiTargetingPackageInfo, options, logger);
                // 当前多个不同的框架引用不同的文件还不能支持，因此随意获取一个打包文件夹即可
                // 为什么？逻辑上不好解决，其次，安装的项目的兼容性无法处理
                // 安装的项目的兼容性无法处理？源代码包有 net45 框架，项目是 net47 框架，如何让项目能兼容使用到 net45 框架？当前没有此生成逻辑 
                var sourcePackingFolder = GetCommonSourcePackingFolder(multiTargetingPackageInfo, logger);
                var packageOutputPath = options.PackageOutputPath;
                if (!string.IsNullOrEmpty(packageOutputPath))
                {
                    packageOutputPath = packageOutputPath.Trim();
                }
                var packageVersion = options.PackageVersion;

                //                logger.Message($@"项目文件 {projectFile}
                //临时文件{intermediateDirectory}
                //输出文件{packageOutputPath}
                //版本{packageVersion}
                //编译的文件{options.CompileFile}
                //资源文件{options.ResourceFile}
                //内容{options.ContentFile}
                //页面{options.Page}
                //SourcePackingDirectory: {options.SourcePackingDirectory}");

                var description = ReadFile(options.DescriptionFile);
                var copyright = ReadFile(options.CopyrightFile);

                var buildProps = new BuildProps(logger)
                {
                    Authors = options.Authors ?? string.Empty,
                    Company = options.Company ?? string.Empty,
                    Owner = options.Owner ?? options.Authors ?? string.Empty,
                    Copyright = copyright,
                    Description = description,
                    //PackageProjectUrl = options.PackageProjectUrl,
                    //RepositoryType = options.RepositoryType,
                    //RepositoryUrl = options.RepositoryUrl,
                    Title = options.Title,
                    PackageIconUrl = options.PackageIconUrl,
                    PackageLicenseUrl = options.PackageLicenseUrl,
                    PackageReleaseNotes = options.PackageReleaseNotesFile,
                    PackageTags = options.PackageTags
                };

                buildProps.SetSourcePackingDirectory(sourcePackingFolder.FullName);

                var packer = new Packer
                (
                    projectFile: projectFile,
                    intermediateDirectory: intermediateDirectory,
                    packageOutputPath: packageOutputPath,
                    packageVersion: packageVersion,
                    // 不再从 options 读取，多个框架的情况下，需要各自获取
                    //compileFile: options.CompileFile,
                    //resourceFile: options.ResourceFile,
                    //contentFile: options.ContentFile,
                    //page: options.Page,
                    //applicationDefinition: options.ApplicationDefinition,
                    //noneFile: options.None,
                    //embeddedResource: options.EmbeddedResource,
                    packageId: options.PackageId,
                    buildProps: buildProps,
                    commonSourcePackingFolder: sourcePackingFolder,
                    multiTargetingPackageInfo: multiTargetingPackageInfo,
                    // 多框架下，每个框架有自己的引用路径
                    //packageReferenceVersion: options.PackageReferenceVersion
                    logger: logger
                );

                packer.Pack();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            string ReadFile(string? file)
            {
                if (string.IsNullOrEmpty(file))
                {
                    return "";
                }

                file = Path.GetFullPath(file!);
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

        /// <summary>
        /// 兼容修复
        /// </summary>
        /// <param name="options"></param>
        private static void FixOptions(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.MultiTargetingPackageInfoFolder))
            {
                // 如果多框架是空的字符串或者是空的，为了后续的判断方便，统一设置为空值
                options.MultiTargetingPackageInfoFolder = null!;
            }
        }

        /// <summary>
        /// 获取通用的 SourcePacking 文件夹，无视框架版本的不同
        /// </summary>
        /// <returns></returns>
        private static DirectoryInfo GetCommonSourcePackingFolder(MultiTargetingPackageInfo multiTargetingPackageInfo, Logger logger)
        {
            // 获取时，需要判断文件夹是否合法
            var fileList = multiTargetingPackageInfo.ValidTargetFrameworkPackageInfoList;

            if (fileList.Count == 0)
            {
                logger.Error($"Can not find any TargetFramework info file from \"{multiTargetingPackageInfo.MultiTargetingPackageInfoFolder.FullName}\"");
                Exit(-1);
            }

            // 如果是单个框架的项目，那么返回即可
            if (fileList.Count == 1)
            {
                return fileList[0].SourcePackingFolder;
            }
            else
            {
                return fileList[0].SourcePackingFolder;
            }
        }

        private static string GetIntermediateDirectory(MultiTargetingPackageInfo multiTargetingPackageInfo, Options options, Logger logger)
        {
            // 多框架和单个框架的逻辑不相同
            var folder = options.MultiTargetingPackageInfoFolder;
            folder = Path.GetFullPath(folder);
            const string packageName = "Package";

            if (string.IsNullOrWhiteSpace(options.TargetFrameworks))
            {
                // 单个框架的项目
                var sourcePackingFolder = GetCommonSourcePackingFolder(multiTargetingPackageInfo, logger);
                // 预期是输出 obj\Debug\SourcePacking\Package 文件
                var intermediateFolder = Path.Combine(sourcePackingFolder.FullName, packageName);
                return intermediateFolder;
            }
            else
            {
                // 多个框架的项目
                // 输出 obj\Debug\SourceYardMultiTargetingPackageInfoFolder\Package 文件夹
                return Path.Combine(folder, packageName);
            }
        }

        private static void Exit(int code)
        {
            Environment.Exit(code);
        }
    }
}
