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
        public Packer(string projectFile, string intermediateDirectory,
            string packageOutputPath, string packageVersion, string compileFile, string resourceFile, string contentFile, string page, string applicationDefinition)
        {
            Logger = new Logger();
            _projectFile = Path.GetFullPath(projectFile);
            _intermediateDirectory = Path.GetFullPath(intermediateDirectory);
            _packageOutputPath = Path.GetFullPath(packageOutputPath);
            _packageVersion = packageVersion;

            PackagedProjectFile = new PackagedProjectFile
            (
                compileFile:           Path.GetFullPath(compileFile),
                resourceFile:          Path.GetFullPath(resourceFile),
                contentFile:           Path.GetFullPath(contentFile),
                page:                  Path.GetFullPath(page),
                applicationDefinition: Path.GetFullPath(applicationDefinition)
            );

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
            PrepareEmptyDirectory(_intermediateDirectory);

            var projectFile = _projectFile;

            var projectName = Path.GetFileNameWithoutExtension(projectFile);

            var projectFolder = Path.GetDirectoryName(projectFile);
            var packingFolder = Path.Combine(_intermediateDirectory, projectName);

            //Directory.Build.props
            var buildProps = GetBuildProps(new DirectoryInfo(projectFolder));

            if (string.IsNullOrWhiteSpace(projectName))
            {
                Logger.Error($"无法从 {projectFile} 解析出正确的项目名称。");
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
                        _packageVersion,
                        _packageOutputPath,
                        packingFolder,
                        PackagedProjectFile
                    )
                    {
                        BuildProps = buildProps,
                    };
                    packer.Pack(context);
                }
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

        private readonly string _projectFile;
        private readonly string _intermediateDirectory;
        private readonly string _packageOutputPath;
        private readonly string _packageVersion;
        private readonly IPackFlow[] _packers;

        private PackagedProjectFile PackagedProjectFile { get; }


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
                Logger.Error(e.Message);
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
                        Logger.Error(e.Message);
                    }
                }
            }
        }
    }
}
