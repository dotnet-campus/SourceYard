using System.IO;
using dotnetCampus.SourceYard.Logger;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.Context
{
    /// <inheritdoc />
    internal class PackingContext : IPackingContext
    {
        public PackingContext(ILogger logger, string selfProjectFile, string projectFile,
            string projectName, string packageVersion, string packageOutputPath, string packingFolder)
        {
            Logger = logger;
            SelfProjectFile = selfProjectFile;
            SelfProjectFolder = Path.GetDirectoryName(selfProjectFile);
            ProjectFile = projectFile;
            ProjectFolder = Path.GetDirectoryName(projectFile);
            ProjectName = projectName;
            PackageId = projectName + ".Source";
            PackageGuid = projectName.Replace(".", "");
            PackingFolder = packingFolder;
            PackageVersion = packageVersion;
            PackageOutputPath = packageOutputPath;
        }

        /// <inheritdoc />
        public ILogger Logger { get; }

        /// <inheritdoc />
        public string SelfProjectFile { get; }

        /// <inheritdoc />
        public string SelfProjectFolder { get; }

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

        public BuildProps BuildProps { get; set; }
    }
}
