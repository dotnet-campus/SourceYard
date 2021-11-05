using System.IO;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.Context
{
    /// <inheritdoc />
    internal class PackingContext : IPackingContext
    {
        public PackingContext(ILogger logger, string projectFile,
            string projectName, string? packageId, string packageVersion, string packageOutputPath, string packingFolder,
            PackagedProjectFile packagedProjectFile)
        {
            Logger = logger;

            ProjectFile = projectFile;
            ProjectFolder = Path.GetDirectoryName(projectFile);
            ProjectName = projectName;
            packageId = packageId?.Trim() ?? "";
            if (string.IsNullOrEmpty(packageId))
            {
                PackageId = projectName + ".Source";
            }
            else
            {
                PackageId = packageId + ".Source";
            }

            PackageGuid = projectName.Replace(".", "");
            PackingFolder = packingFolder;
            PackagedProjectFile = packagedProjectFile;
            PackageVersion = packageVersion;
            PackageOutputPath = packageOutputPath;
        }

        /// <inheritdoc />
        public ILogger Logger { get; }

        /// <inheritdoc />
        public string ProjectFile { get; }

        /// <inheritdoc />
        public string ProjectFolder { get; }

        /// <inheritdoc />
        public string ProjectName { get; }

        /// <inheritdoc />
        public string PackageId { get; set; }

        /// <inheritdoc />
        public string PackageGuid { get; set; }

        /// <inheritdoc />
        public string PackageVersion { get; }

        /// <inheritdoc />
        public string PackageOutputPath { get; }

        /// <inheritdoc />
        public string PackingFolder { get; }

        public PackagedProjectFile PackagedProjectFile { get; }

        public BuildProps BuildProps { get; set; } = null!;
    }
}
