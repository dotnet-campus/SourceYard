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
        /// <inheritdoc />
        public PackagedProjectFile(string compileFile, string resourceFile, string contentFile, string page, string applicationDefinition, string noneFile, string embeddedResource, string projectFolder)
        {
            _projectFolder = projectFolder;
            ApplicationDefinition = applicationDefinition;

            CompileFileList = GetFileList(compileFile);
            ResourceFileList = GetFileList(resourceFile);
            ContentFileList = GetFileList(contentFile);
            PageList = GetFileList(page);
            NoneFileList = GetFileList(noneFile);
            EmbeddedResourceList = GetFileList(embeddedResource);
        }

        /// <summary>
        /// 需要做源码包的项目的编译的文件
        /// </summary>
        public IReadOnlyList<string> CompileFileList { get; }

        /// <summary>
        /// 需要做源码包的项目的资源文件
        /// </summary>
        public IReadOnlyList<string> ResourceFileList { get; }

        /// <summary>
        /// 需要做源码包项目的文件
        /// </summary>
        public IReadOnlyList<string> ContentFileList { get; }

        /// <summary>
        /// 需要做源码包项目的页面
        /// </summary>
        public IReadOnlyList<string> PageList { get; }

        /// <summary>
        /// 嵌入文件
        /// </summary>
        public IReadOnlyList<string> EmbeddedResourceList { get; }

        /// <summary>
        /// 需要做源码包项目的文件
        /// </summary>
        private string ApplicationDefinition { get; }

        public IReadOnlyList<string> NoneFileList { get; }

        public IReadOnlyList<string> GetAllFile()
        {
            return new CombineReadonlyList<string>(CompileFileList, ResourceFileList, ContentFileList, PageList,
                NoneFileList, EmbeddedResourceList);
        }

        private List<string> GetFileList(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                return new List<string>();
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

            return fileList;
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

            fileList.RemoveAll(temp => IgnoreFileEndList.Any(t => temp.EndsWith(t, StringComparison.OrdinalIgnoreCase)));

            return fileList;
        }

        /// <summary>
        /// 项目文件夹
        /// </summary>
        private readonly string _projectFolder;

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
            ".csproj.DotSettings", ".suo", ".user", ".sln.docstates", ".nupkg"
        };
    }
}