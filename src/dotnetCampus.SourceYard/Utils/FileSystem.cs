using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using dotnetCampus.SourceYard.Context;

namespace dotnetCampus.SourceYard.Utils
{
    internal static class FileSystem
    {
        /// <summary>
        /// 复制文件列表
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="targetFolder"></param>
        /// <param name="fileList"></param>
        public static void CopyFileList(string sourceFolder, string targetFolder, List<SourceYardPackageFile> fileList)
        {
            foreach (var file in fileList)
            {
                //var relativePath = MakeRelativePath(sourceFolder, file);
                var targetFile = Path.GetFullPath(Path.Combine(targetFolder, file.SourcePackagePath));
                var directory = Path.GetDirectoryName(targetFile);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.Copy(file.SourceFile.FullName, targetFile, true);
            }
        }

        internal static void CopyFolderContents(string sourceFolder, string targetFolder,
            Func<string, string> nameConverter = null, IList<string> excludes = null)
        {
            var sourceDirectory = new DirectoryInfo(sourceFolder);
            foreach (var directory in sourceDirectory.EnumerateDirectories().Where(
                x => excludes?.Contains(x.Name) != true))
            {
                CopyFiles(directory.FullName, Path.Combine(targetFolder, directory.Name), SearchOption.AllDirectories,
                    nameConverter);
            }

            CopyFiles(sourceDirectory.FullName, targetFolder, SearchOption.TopDirectoryOnly, nameConverter);
        }

        /// <summary>
        /// 忽略的文件夹列表
        /// </summary>
        internal static IList<string> IgnoreFolderList { set; get; } = new List<string>();

        /// <summary>
        /// 忽略的文件后缀列表
        /// </summary>
        internal static IList<string> IgnoreFileEndList { set; get; } = new List<string>();

        private static void CopyFiles(string sourceFolder, string targetFolder, SearchOption searchOption,
            Func<string, string> nameConverter = null)
        {
            foreach (var file in new DirectoryInfo(sourceFolder).EnumerateFiles("*", searchOption))
            {
                // 如果是忽略的文件夹，就直接忽略
                if (IsIgnoreFolder(file) || IsIgnoredFile(file))
                {
                    continue;
                }

                var relativePath = MakeRelativePath(sourceFolder, file.FullName);
                var targetFile = Path.GetFullPath(Path.Combine(targetFolder, relativePath));
                var directory = Path.GetDirectoryName(targetFile);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var fileName = Path.GetFileName(targetFile);
                if (nameConverter != null)
                {
                    fileName = nameConverter(fileName);
                    targetFile = Path.Combine(Path.GetDirectoryName(targetFile) ?? throw new InvalidOperationException(), fileName);
                }

                File.Copy(file.FullName, targetFile, true);
            }
        }

        /// <summary>
        /// 是否是忽略的文件
        /// </summary>
        /// <param name="file"></param>
        private static bool IsIgnoredFile(FileInfo file)
        {
            var name = file.Name;

            if (IgnoreFileEndList == null)
            {
                return false;
            }

            foreach (var temp in IgnoreFileEndList)
            {
                if (name.EndsWith(temp, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsIgnoreFolder(FileInfo file)
        {
            var directory = file.Directory;
            Debug.Assert(directory != null);
            var name = directory?.Name;

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (IgnoreFolderList == null)
            {
                return false;
            }

            foreach (var temp in IgnoreFolderList)
            {
                if (name.Equals(temp, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                throw new ArgumentNullException(nameof(fromPath));
            }

            if (string.IsNullOrEmpty(toPath))
            {
                throw new ArgumentNullException(nameof(toPath));
            }

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme)
            {
                // 不是同一种路径，无法转换成相对路径。
                return toPath;
            }

            if (fromUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase)
                && !fromPath.EndsWith("/") && !fromPath.EndsWith("\\"))
            {
                // 如果是文件系统，则视来源路径为文件夹。
                fromUri = new Uri(fromPath + Path.DirectorySeparatorChar);
            }

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }
    }
}