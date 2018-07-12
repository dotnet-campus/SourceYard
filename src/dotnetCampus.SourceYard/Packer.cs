using System;
using System.IO;
using System.Linq;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.Logger;
using dotnetCampus.SourceYard.PackFlow;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard
{
    internal class Packer
    {
        private readonly ILogger _logger;
        private readonly string _selfProjectFile;
        private readonly string _packageOutputPath;
        private readonly string _packageVersion;
        private readonly string _intermediateOutputPath;
        private readonly string[] _projectFiles;
        private readonly IPackFlow[] _packers;

        public Packer(string selfProjectFile, string intermediateOutputPath, string packageOutputPath,
            string packageVersion, string[] projectFiles)
        {
            _logger = new Logger.Logger();
            _selfProjectFile = selfProjectFile;
            _packageOutputPath = Path.GetFullPath(packageOutputPath);
            _packageVersion = packageVersion;
            _intermediateOutputPath = Path.GetFullPath(intermediateOutputPath);
            _projectFiles = projectFiles.Select(Path.GetFullPath).ToArray();

            _packers = new IPackFlow[]
            {
                new SourcePacker(),
                new AssetsPacker(),
                new ItemGroupPacker(),
                new NuspecFileGenerator(),
                new NuGetPacker(),
            };
        }

        internal void Pack()
        {
            PrepareEmptyDirectory(_intermediateOutputPath);

            foreach (var projectFile in _projectFiles)
            {
                var projectName = Path.GetFileNameWithoutExtension(projectFile);
                var projectFolder = Path.GetDirectoryName(projectFile);
                var packingFolder = Path.Combine(_intermediateOutputPath, projectName);

                //Directory.Build.props
                var buildProps = GetBuildProps(new DirectoryInfo(projectFolder));

                if (string.IsNullOrWhiteSpace(projectName))
                {
                    _logger.Error($"无法从 {projectFile} 解析出正确的项目名称。");
                }

                IPackFlow current = null;
                try
                {
                    foreach (var packer in _packers)
                    {
                        current = packer;
                        var context = new PackingContext(_logger,
                            _selfProjectFile,
                            projectFile,
                            projectName,
                            _packageVersion,
                            _packageOutputPath,
                            packingFolder)
                        {
                            BuildProps = buildProps,
                        };
                        packer.Pack(context);
                    }
                }
                catch (PackingException ex)
                {
                    _logger.Error(ex);
                }
                catch (Exception ex)
                {
                    _logger.Error($"生成源码包: {current?.GetType().Name}: {ex}");
                }
            }
        }

        private BuildProps GetBuildProps(DirectoryInfo projectFolder)
        {
            var file = "Directory.Build.props";

            try
            {
                while (projectFolder != null)
                {
                    if (ContainsFile())
                    {
                        return BuildProps.Parse(new FileInfo(Path.Combine(projectFolder.FullName, file)));
                    }

                    projectFolder = projectFolder.Parent;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }

            return null;

            bool ContainsFile()
            {
                try
                {
                    return projectFolder.GetFiles().Any(temp => string.Equals(temp.Name, "dotnetCampus.SourceYard.sln"));
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

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
                        _logger.Error(e.Message);
                    }
                }
            }
        }
    }
}