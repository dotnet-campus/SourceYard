using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.PackFlow
{
    internal class SelfPacker : IPackFlow
    {
        public void Pack(IPackingContext context)
        {
            var current = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            while (current != null && !current.Name.Equals("tools", StringComparison.OrdinalIgnoreCase))
            {
                current = current.Parent;
            }
            if (current is null)
            {
                return;
            }

            MoveFolder(current.FullName, context.PackingFolder);
        }

        public string MoveFolder(string source, string targetFolder)
        {
            var sourceFolder = new DirectoryInfo(source);

            Log("开始移动文件夹");

            var installFolder = Path.Combine(targetFolder, sourceFolder.Name);
            try
            {
                if (Directory.Exists(installFolder))
                {
                    Log("发现" + installFolder + "存在，开始删除文件");
                    DeleteDirectory(installFolder);
                    Log("自动更新插件删除文件成功");
                }
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }

            try
            {
                if (Path.GetPathRoot(sourceFolder.FullName) == Path.GetPathRoot(installFolder))
                {
                    Log("在相同驱动器，移动文件到安装");
                    Directory.CreateDirectory(Path.GetDirectoryName(installFolder));
                    Directory.Move(sourceFolder.FullName, installFolder);
                }
                else
                {
                    Log("在不相同驱动器，使用复制文件到安装");
                    DirectoryCopy(sourceFolder, installFolder);
                }
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }

            return installFolder;
        }

        private void DirectoryCopy(DirectoryInfo sourceDirectory, string destDirName)
        {
            Log($"从{sourceDirectory.FullName}复制到{destDirName}");

            // Get the subdirectories for the specified directory.

            if (!sourceDirectory.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirectory.FullName);
            }

            DirectoryInfo[] dirs = sourceDirectory.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = sourceDirectory.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.

            foreach (DirectoryInfo temp in dirs)
            {
                string tempPath = Path.Combine(destDirName, temp.Name);
                DirectoryCopy(temp, tempPath);
            }
        }

        private static void DeleteDirectory(string directory)
        {
            DeleteDirectory(directory, 0);

            void DeleteDirectory(string d, int depth)
            {
                if (!Directory.Exists(d))
                {
                    return;
                }

                var sourceFolder = new DirectoryInfo(d);
                foreach (var fileInfo in sourceFolder.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
                {
                    File.Delete(fileInfo.FullName);
                }

                foreach (var directoryInfo in sourceFolder.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    var back = string.Join("\\", Enumerable.Repeat("..", depth));
                    DeleteDirectory(directoryInfo.FullName, depth + 1);
                }

                Directory.Delete(d);
            }
        }

        private void Log(string _)
        {
        }
    }
}
