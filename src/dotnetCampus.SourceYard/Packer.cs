using System;
using System.IO;
using System.Linq;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.PackFlow;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard
{
    internal class Packer
    {
        public Packer
        (
            string projectFile,
            string intermediateDirectory,
            string packageOutputPath,
            string packageVersion,
            string? packageId,
            BuildProps buildProps,
            DirectoryInfo commonSourcePackingFolder,
            MultiTargetingPackageInfo multiTargetingPackageInfo,
            Logger logger
        )
        {
            Logger = logger;

            Logger.Message(message: "初始化打包");

            if (string.IsNullOrEmpty(value: projectFile) || !File.Exists(path: projectFile))
            {
                Logger.Error(message: $"无法从{projectFile}找到项目文件");
                return;
            }

            if (string.IsNullOrEmpty(value: intermediateDirectory))
            {
                // 这时的文件夹可以不存在
                Logger.Error(message: "无法解析文件夹 " + intermediateDirectory);
                return;
            }

            if (string.IsNullOrEmpty(value: packageOutputPath))
            {
                Logger.Error(message: "打包输出文件夹不能为空");
                return;
            }

            if (string.IsNullOrEmpty(value: packageVersion))
            {
                Logger.Error(message: "打包版本不能为空");
                return;
            }

            _projectFile = Path.GetFullPath(path: projectFile);
            _intermediateDirectory = Path.GetFullPath(path: intermediateDirectory);
            _packageOutputPath = Path.GetFullPath(path: packageOutputPath);
            _packageVersion = packageVersion;
            _multiTargetingPackageInfo = multiTargetingPackageInfo;
            //_packageReferenceVersion = Path.GetFullPath(path: packageReferenceVersion);
            BuildProps = buildProps;
            PackageId = packageId;

            PackagedProjectFile = new PackagedProjectFile
            (
                commonSourcePackingFolder,
                //compileFile: GetFile(file: compileFile),
                //resourceFile: GetFile(file: resourceFile),
                //contentFile: GetFile(file: contentFile),
                //embeddedResource: GetFile(file: embeddedResource),
                //page: GetFile(file: page),
                //applicationDefinition: GetFile(file: applicationDefinition),
                //noneFile: GetFile(file: noneFile),
                projectFolder: Path.GetDirectoryName(path: _projectFile)!,
                buildProps: buildProps
            );

            _packers = new IPackFlow[]
            {
                new SourcePacker(),
                new AssetsPacker(),
                new ItemGroupPacker(),
                new NuspecFileGenerator(),
                new NuGetPacker(),
            };

            Logger.Message(message: "初始化打包完成");
        }

        internal void Pack()
        {
            Logger.Message("开始打包");

            var packingFolder = _intermediateDirectory;
            PrepareEmptyDirectory(packingFolder);

            var projectFile = _projectFile;

            var projectName = Path.GetFileNameWithoutExtension(projectFile);

            if (string.IsNullOrEmpty(projectName))
            {
                Logger.Error("从项目文件无法拿到项目文件名 " + projectFile);
                return;
            }

            var projectFolder = Path.GetDirectoryName(projectFile);

            if (string.IsNullOrEmpty(projectFolder))
            {
                Logger.Error("无法拿到项目文件所在文件夹");
                return;
            }

            var buildProps = BuildProps;

            IPackFlow? current = null;
            try
            {
                foreach (var packer in _packers)
                {
                    current = packer;
                    var context = new PackingContext
                    (
                        Logger,
                        projectFile,
                        projectName,
                        PackageId,
                        _packageVersion,
                        _packageOutputPath,
                        packingFolder,
                        PackagedProjectFile,
                        _multiTargetingPackageInfo
                    )
                    {
                        BuildProps = buildProps,
                    };
                    packer.Pack(context);
                }

                Logger.Message("打包完成");
            }
            catch (PackingException ex)
            {
                Logger.Error(ex);
            }
            catch (Exception ex)
            {
                Logger.Error($"生成源码包: {current?.GetType().Name}: {ex}");
            }
        }

        private ILogger Logger { get; }
        private string? PackageId { get; }

        private readonly string _projectFile = null!;
        private readonly string _intermediateDirectory = null!;

        /// <summary>
        /// 最后输出的文件夹
        /// </summary>
        private readonly string _packageOutputPath = null!;

        private readonly string _packageVersion = null!;
        private readonly MultiTargetingPackageInfo _multiTargetingPackageInfo;

        private readonly IPackFlow[] _packers = null!;

        private PackagedProjectFile PackagedProjectFile { get; } = null!;
        private BuildProps BuildProps { get; } = null!;

        /// <summary>
        /// 准备一个空白的文件夹用来放文件
        /// </summary>
        /// <param name="directory"></param>
        private void PrepareEmptyDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            else
            {
                foreach (var fileSystemInfo in new DirectoryInfo(directory)
                    .EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        if (fileSystemInfo is DirectoryInfo directoryInfo)
                        {
                            directoryInfo.Delete(true);
                        }
                        else if (fileSystemInfo is FileInfo fileInfo)
                        {
                            fileInfo.Delete();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.Message);
                    }
                }
            }
        }
    }
}