using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.Context
{
    /// <summary>
    /// 被打包项目的文件，包括编译的文件资源文件
    /// </summary>
    class PackagedProjectFile
    {
        public PackagedProjectFile(string commonSourcePackingFolder, string projectFolder,
            BuildProps buildProps)
        {
            // 通过 commonSourcePackingFolder 可以拿到对应的输出文件路径
            string compileFile = GetFile("CompileFile.txt");
            string resourceFile = GetFile("ResourceFile.txt");
            string contentFile = GetFile("ContentFile.txt");
            string page = GetFile("PageFile.txt");
            string applicationDefinition = GetFile("ApplicationDefinitionFile.txt");
            string noneFile = GetFile("NoneFile.txt");
            string embeddedResource = GetFile("EmbeddedResourceFile.txt");

            _projectFolder = projectFolder;
            _buildProps = buildProps;
            ApplicationDefinition = applicationDefinition;

            CompileFileList = GetSourceYardPackageFileList(compileFile, SourceYardCompilePackageFile);
            ResourceFileList = GetSourceYardPackageFileList(resourceFile, SourceYardResourcePackageFile);
            ContentFileList = GetSourceYardPackageFileList(contentFile, SourceYardContentPackageFile);
            PageList = GetFileList(page);
            NoneFileList = GetSourceYardPackageFileList(noneFile, SourceYardNonePackageFile);
            EmbeddedResourceList = GetSourceYardPackageFileList(embeddedResource, SourceYardEmbeddedResourcePackageFile);

            string GetFile(string fileName)
            {
                return Path.Combine(commonSourcePackingFolder, fileName);
            }
        }

        private const string
            SourceYardCompilePackageFile = "SourceYardCompilePackageFile.txt",
            SourceYardResourcePackageFile = "SourceYardResourcePackageFile.txt",
            SourceYardContentPackageFile = "SourceYardContentPackageFile.txt",
            SourceYardNonePackageFile = "SourceYardNonePackageFile.txt",
            SourceYardEmbeddedResourcePackageFile = "SourceYardEmbeddedResourcePackageFile.txt";

        /// <summary>
        /// 需要做源码包的项目的编译的文件
        /// </summary>
        public IReadOnlyList<SourceYardPackageFile> CompileFileList { get; }

        /// <summary>
        /// 需要做源码包的项目的资源文件
        /// </summary>
        public IReadOnlyList<SourceYardPackageFile> ResourceFileList { get; }

        /// <summary>
        /// 需要做源码包项目的文件
        /// </summary>
        public IReadOnlyList<SourceYardPackageFile> ContentFileList { get; }

        /// <summary>
        /// 需要做源码包项目的页面
        /// </summary>
        public IReadOnlyList<SourceYardPackageFile> PageList { get; }

        /// <summary>
        /// 嵌入文件
        /// </summary>
        public IReadOnlyList<SourceYardPackageFile> EmbeddedResourceList { get; }

        /// <summary>
        /// 需要做源码包项目的文件
        /// </summary>
        private string ApplicationDefinition { get; }

        public IReadOnlyList<SourceYardPackageFile> NoneFileList { get; }

        public IReadOnlyList<SourceYardPackageFile> GetAllFile()
        {
            return new CombineReadonlyList<SourceYardPackageFile>(CompileFileList, ResourceFileList, ContentFileList,
                PageList,
                NoneFileList, EmbeddedResourceList);
        }

        private List<SourceYardPackageFile> GetSourceYardPackageFileList(string file,
            string sourceYardPackageFile)
        {
            sourceYardPackageFile = Path.Combine(_buildProps.SourcePackingDirectory, sourceYardPackageFile);
            var sourceYardPackageFileList = ParseSourceYardPackageFile(sourceYardPackageFile);

            var fileList = GetFileList(file);
            return new CombineReadonlyList<SourceYardPackageFile>(sourceYardPackageFileList, fileList).Distinct(new SourceYardPackageFileEqualityComparer()).ToList();
        }

        private List<SourceYardPackageFile> ParseSourceYardPackageFile(string sourceYardPackageFile)
        {
            var sourceYardPackageFileList = new List<SourceYardPackageFile>();
            var text = File.ReadAllText(sourceYardPackageFile);

            foreach (var line in text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                var package = line.Split('|');
                if (package.Length == 2)
                {
                    var sourceFile = package[0];
                    var packagePath = package[1];

                    if (!string.IsNullOrEmpty(sourceFile))
                    {
                        sourceYardPackageFileList.Add(new SourceYardPackageFile(new FileInfo(sourceFile), packagePath));
                    }
                }
            }

            return sourceYardPackageFileList;
        }

        private List<SourceYardPackageFile> GetFileList(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                return new List<SourceYardPackageFile>();
            }

            var fileList = File.ReadAllLines(file).ToList();

            fileList = RemoveTempFile(fileList);

            for (var i = 0; i < fileList.Count; i++)
            {
                var fullPath = Path.GetFullPath(fileList[i]);

                // 如果文件放在项目文件上面，如 Foo\F1\F.csproj 引用文件 Foo\12.txt 那么此时就不添加 12.txt 的引用
                // 原因是将 F.csproj 所在文件夹放在源代码包的 src 文件夹里面，因此 F1 文件夹是最上层文件夹
                if (fullPath.IndexOf(_projectFolder, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    fileList.RemoveAt(i);
                    i--;
                }
            }

            return fileList.Select(temp => new SourceYardPackageFile(new FileInfo(temp), temp)).ToList();
        }

        private List<string> RemoveTempFile(List<string> fileList)
        {
            fileList.RemoveAll
            (
                temp => IgnoreFolderList.Any(t => temp.StartsWith(t, StringComparison.OrdinalIgnoreCase))
            );

            fileList.RemoveAll(temp =>
            {
                var pathRoot = Path.GetPathRoot(temp);
                if (!string.IsNullOrEmpty(pathRoot))
                {
                    return temp.StartsWith(pathRoot, StringComparison.OrdinalIgnoreCase);
                }

                return false;
            });

            fileList.RemoveAll(temp =>
                IgnoreFileEndList.Any(t => temp.EndsWith(t, StringComparison.OrdinalIgnoreCase)));

            // 忽略被排除的文件
            fileList.RemoveAll(temp =>
                _buildProps.SourceYardExcludeFileItemList.Contains(temp, StringComparer.OrdinalIgnoreCase));

            return fileList;
        }

        /// <summary>
        /// 项目文件夹
        /// </summary>
        private readonly string _projectFolder;

        private readonly BuildProps _buildProps;

        /// <summary>
        /// 忽略的文件夹列表
        /// </summary>
        private static IList<string> IgnoreFolderList { get; } = new List<string>()
        {
            ".vs", "bin", "obj", ".git", "x64", "x86"
        };

        /// <summary>
        /// 忽略的文件后缀列表
        /// </summary>
        private static IList<string> IgnoreFileEndList { get; } = new List<string>()
        {
            ".csproj.DotSettings", ".suo", ".user", ".sln.docstates",
            ".nupkg",
            // 忽略原因请看 https://github.com/dotnet-campus/SourceYard/issues/98
            "launchSettings.json"
        };

        class SourceYardPackageFileEqualityComparer : IEqualityComparer<SourceYardPackageFile>
        {
            public bool Equals(SourceYardPackageFile x, SourceYardPackageFile y)
            {
                return x.SourceFile.FullName.Equals(y.SourceFile.FullName);
            }

            public int GetHashCode(SourceYardPackageFile obj)
            {
                return obj.SourceFile.FullName.GetHashCode();
            }
        }
    }
}