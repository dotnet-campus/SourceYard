﻿using System;
using System.IO;
using System.Linq;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.PackFlow;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard
{
    internal class Packer
    {
        public Packer(string projectFile, string intermediateDirectory,
            string packageOutputPath, string packageVersion, string compileFile, string resourceFile,
            string contentFile, string page, string applicationDefinition, string noneFile, string embeddedResource,
            BuildProps buildProps, string packageId, string packageReferenceVersion,
            string rootNamespace)
        {
            Logger = new Logger();

            Logger.Message("初始化打包");

            if (string.IsNullOrEmpty(projectFile) || !File.Exists(projectFile))
            {
                Logger.Error($"无法从{projectFile}找到项目文件");
                return;
            }

            if (string.IsNullOrEmpty(intermediateDirectory))
            {
                // 这时的文件夹可以不存在
                Logger.Error("无法解析文件夹 " + intermediateDirectory);
                return;
            }

            if (string.IsNullOrEmpty(packageOutputPath))
            {
                Logger.Error("打包输出文件夹不能为空");
                return;
            }

            if (string.IsNullOrEmpty(packageVersion))
            {
                Logger.Error("打包版本不能为空");
                return;
            }

            _projectFile = Path.GetFullPath(projectFile);
            _intermediateDirectory = Path.GetFullPath(intermediateDirectory);
            _packageOutputPath = Path.GetFullPath(packageOutputPath);
            _packageVersion = packageVersion;
            _packageReferenceVersion = Path.GetFullPath(packageReferenceVersion);
            BuildProps = buildProps;
            PackageId = packageId;
            _rootNamespace = rootNamespace;

            PackagedProjectFile = new PackagedProjectFile
            (
                compileFile: GetFile(compileFile),
                resourceFile: GetFile(resourceFile),
                contentFile: GetFile(contentFile),
                embeddedResource: GetFile(embeddedResource),
                page: GetFile(page),
                applicationDefinition: GetFile(applicationDefinition),
                noneFile: GetFile(noneFile)
            );

            _packers = new IPackFlow[]
            {
                new SelfPacker(),
                new SourcePacker(),
                new AssetsPacker(),
                new ItemGroupPacker(),
                new ProjectPropertiesGenerator(),
                new NuspecFileGenerator(),
                new NuGetPacker(),
            };

            Logger.Message("初始化打包完成");
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

            //Directory.Build.props
            var buildProps = BuildProps;

            if (string.IsNullOrWhiteSpace(projectName))
            {
                Logger.Error($"无法从 {projectFile} 解析出正确的项目名称。");
                return;
            }

            IPackFlow current = null;
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
                        _packageReferenceVersion,
                        _rootNamespace
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
        private string PackageId { get; }

        private readonly string _projectFile;
        private readonly string _intermediateDirectory;

        /// <summary>
        /// 最后输出的文件夹
        /// </summary>
        private readonly string _packageOutputPath;

        private readonly string _packageVersion;
        private readonly string _packageReferenceVersion;
        private readonly IPackFlow[] _packers;
        private readonly string _rootNamespace;

        private PackagedProjectFile PackagedProjectFile { get; }
        private BuildProps BuildProps { get; }

        private static string GetFile(string file)
        {
            return string.IsNullOrWhiteSpace(file) ? "" : Path.GetFullPath(file);
        }

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