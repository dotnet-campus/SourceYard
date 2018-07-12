using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dotnetCampus.SourceYard.Utils
{
    internal static class FileSystem
    {
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

        private static void CopyFiles(string sourceFolder, string targetFolder, SearchOption searchOption,
            Func<string, string> nameConverter = null)
        {
            foreach (var file in new DirectoryInfo(sourceFolder).EnumerateFiles("*", searchOption))
            {
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
                    targetFile = Path.Combine(Path.GetDirectoryName(targetFile), fileName);
                }

                File.Copy(file.FullName, targetFile, true);
            }
        }

        private static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

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
