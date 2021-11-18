using dotnetCampus.Cli;

namespace dotnetCampus.SourceYard.Cli
{
    internal class Options
    {
        /// <summary>
        /// 项目文件所在的路径
        /// </summary>
        [Option('p', "project", Description = "The full path of the project file.")]
        public string ProjectFile { get; set; } = null!;

        ///// <summary>
        ///// 临时的文件夹的路径
        ///// </summary>
        //[Option('i', "intermediate-directory", Description = "The relative path of the project intermediate output path, commonly the 'obj' folder and value in 'obj'.")]
        //public string IntermediateDirectory { get; set; } = null!;

        /// <summary>
        /// 打包输出的文件夹
        /// </summary>
        [Option('n', "package-output-path", Description = "The package output full path of the project.")]
        public string PackageOutputPath { get; set; } = null!;

        /// <summary>
        /// 当前项目的打包版本
        /// </summary>
        [Option('v', "package-version",
            // 默认不需要加上打包版本
            Description = "The package version value.")]
        public string PackageVersion { get; set; } = "1.0.0";

        ///// <summary>
        ///// 项目进行编译的文件的列表，因为参数太多，需要将参数写到文件
        ///// </summary>
        //[Option(longName: "Compile", Description = "编译的文件")]
        //public string? CompileFile { get; set; }

        ///// <summary>
        ///// 存放资源文件参数的文件
        ///// </summary>
        //[Option(longName: "Resource", Description = "资源文件")]
        //public string? ResourceFile { get; set; }

        //[Option(longName: "Content")]
        //public string? ContentFile { get; set; }

        //[Option(longName: "Page")]
        //public string? Page { get; set; }

        //[Option(longName: "None")]
        //public string? None { get; set; }

        //[Option(longName: "EmbeddedResource")]
        //public string? EmbeddedResource { get; set; }

        /// <summary>
        /// 打包的公司内容
        /// </summary>
        [Option(longName: "Company")]
        public string? Company { get; set; }

        [Option(longName: "Authors")]
        public string? Authors { get; set; }

        // 内容放在 SourceProjectPackageFile 文件
        //[Option(longName: "RepositoryUrl")]
        //public string RepositoryUrl { get; set; }

        //[Option(longName: "RepositoryType")]
        //public string RepositoryType { get; set; }

        //[Option(longName: "PackageProjectUrl")]
        //public string PackageProjectUrl { get; set; }

        /// <summary>
        /// 因为 Copyright 可能很长，有空格，写文件
        /// </summary>
        [Option(longName: "CopyrightFile")]
        public string? CopyrightFile { get; set; }

        [Option(longName: "DescriptionFile")]
        public string? DescriptionFile { get; set; }

        //[Option(longName: "ApplicationDefinition")]
        //public string? ApplicationDefinition { get; set; }

        [Option(longName: "Owner")]
        public string? Owner { get; set; }

        [Option(longName: "Title")]
        public string? Title { get; set; }

        [Option(longName: "PackageLicenseUrl")]
        public string? PackageLicenseUrl { get; set; }

        [Option(longName: "PackageIconUrl")]
        public string? PackageIconUrl { get; set; }

        [Option(longName: "PackageReleaseNotesFile")]
        public string? PackageReleaseNotesFile { get; set; }

        [Option(longName: "PackageTags")]
        public string? PackageTags { get; set; }

        [Option(longName: "PackageId")] 
        public string? PackageId { get; set; }

        //[Option(longName: "PackageReferenceVersion")]
        //public string? PackageReferenceVersion { get; set; }

        ///// <summary>
        ///// 打包用到的文件夹
        ///// </summary>
        //[Option(longName: "SourcePackingDirectory")]
        //public string SourcePackingDirectory { get; set; } = null!;

        [Option(longName: "TargetFramework")]
        public string? TargetFramework { get; set; }

        [Option(longName: "TargetFrameworks")]
        public string? TargetFrameworks { get; set; }

        /// <summary>
        /// 多框架信息存放的文件夹，即使是单框架项目也会存在此文件夹
        /// </summary>
        [Option("MultiTargetingPackageInfoFolder")]
        public string MultiTargetingPackageInfoFolder { set; get; } = null!;
    }
}
