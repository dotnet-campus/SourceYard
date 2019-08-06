using System;
using System.IO;
using System.IO.Compression;
using dotnetCampus.SourceYard.Context;

namespace dotnetCampus.SourceYard.PackFlow
{
    /// <summary>
    /// 将源项目的一些属性存起来，这样可以在目标项目中使用。
    /// </summary>
    internal class ProjectPropertiesGenerator : IPackFlow
    {
        public void Pack(IPackingContext context)
        {
            var configsFolder = Path.Combine(context.PackingFolder, "configs");
            Directory.CreateDirectory(configsFolder);
            var configsFile = Path.Combine(configsFolder, "Project.txt");
            var configsContent = $@">
RootNamespace
{File.ReadAllText(context.RootNamespace)}
>";

            const int retryCount = 10;
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    File.WriteAllText(configsFile, configsContent);
                    break;
                }
                catch (IOException)
                {
                    if (i == retryCount - 1)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
