using System.Collections.Generic;
using System.IO;
using System.Linq;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;

namespace dotnetCampus.SourceYard.Utils
{
    /// <summary>
    /// 构建时用到的属性
    /// </summary>
    public class BuildProps
    {
        internal BuildProps(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     公司
        /// </summary>
        public string Company
        {
            get
            {
                if (string.IsNullOrEmpty(_company))
                {
                    _company = "dotnet campus";
                }

                return _company!;
            }
            set => _company = value;
        }

        /// <summary>
        ///     作者
        /// </summary>
        public string Authors
        {
            get
            {
                if (string.IsNullOrEmpty(_authors))
                {
                    _authors = Owner;
                }

                return _authors!;
            }
            set => _authors = value;
        }

        /// <summary>
        ///     仓库地址
        /// </summary>
        /// 在 SourceProjectPackageFile.txt 设置
        public string? RepositoryUrl { get; private set; }

        public string? RepositoryType { get; private set; }

        /// <summary>
        ///     项目地址
        /// </summary>
        /// 在 SourceProjectPackageFile.txt 设置
        public string? PackageProjectUrl { get; private set; }

        /// <summary>
        ///     版权
        /// </summary>
        public string? Copyright { get; set; }

        /// <summary>
        ///     描述
        /// </summary>
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    _description = "This package is built by SourceYard";
                }

                return _description!;
            }
            set => _description = value;
        }

        public string Owner
        {
            get
            {
                if (string.IsNullOrEmpty(_owner))
                {
                    _owner = "dotnet campus";
                }

                return _owner!;
            }
            set => _owner = value;
        }

        public string? Title { get; set; }

        public string? PackageLicenseUrl { get; set; }

        public string? PackageIconUrl { get; set; }

        public string? PackageReleaseNotes { get; set; }

        public string? PackageTags { get; set; }

        /// <summary>
        /// 打包用到的文件夹
        /// </summary>
        public string SourcePackingDirectory { get; private set; } = null!;

        private string? _authors;
        private string? _company;

        private string? _description;
        private string? _owner;

        /// <summary>
        /// 设置打包用到的文件夹，设置时将会自动读取文件
        /// </summary>
        /// <param name="packingDirectory"></param>
        public void SetSourcePackingDirectory(string packingDirectory)
        {
            _logger.Message($"BuildProps SourcePackingDirectory={packingDirectory}");
            SourcePackingDirectory = packingDirectory;

            // 更多信息读取
            var sourceProjectPackageFile = Path.Combine(packingDirectory, "SourceProjectPackageFile.txt");

            // 这个文件里面存放使用 fkv 配置
            if (File.Exists(sourceProjectPackageFile))
            {
                var fileConfigurationRepo = ConfigurationFactory.FromFile(sourceProjectPackageFile);

                IAppConfigurator appConfigurator = fileConfigurationRepo.CreateAppConfigurator();
                var configuration = appConfigurator.Default;

                // ReSharper disable ExplicitCallerInfoArgument
                RepositoryType = configuration.GetValue("RepositoryType")?.Trim() ?? string.Empty;

                PackageProjectUrl = configuration.GetValue("PackageProjectUrl")?.Trim() ?? string.Empty;

                RepositoryUrl = configuration.GetValue("RepositoryUrl")?.Trim() ?? string.Empty;
            }

            // 安装的源代码包列表
            const string sourceYardPackageReferenceFile = "SourceYardPackageReferenceFile.txt";
            SourceYardPackageReferenceList = GetList(sourceYardPackageReferenceFile);

            // 排除的依赖引用列表
            const string sourceYardExcludePackageReferenceFile =
                "SourceYardExcludePackageReferenceFile.txt";
            SourceYardExcludePackageReferenceList = GetList(sourceYardExcludePackageReferenceFile);

            // 排除的文件引用列表，这些文件将不会被打入源代码包
            const string sourceYardExcludeFileItemFile = "SourceYardExcludeFileItemFile.txt";
            SourceYardExcludeFileItemList = GetList(sourceYardExcludeFileItemFile);
        }

        /// <summary>
        /// 排除的文件引用列表
        /// </summary>
        /// 不要在 SourceYard 里面包含的文件项，这些文件项将在打包时被排除
        /// 例如有一些文档是写入到项目里面，期望打包的时候，不要将这些文档作为源代码包的引用，以免被输出，或者干扰调试
        /// 这里面存放的是相对项目的相对路径的文件列表
        public List<string> SourceYardExcludeFileItemList { get; private set; } = null!;

        /// <summary>
        /// 排除的依赖引用列表
        /// </summary>
        /// 有某些 NuGet 引用不想作为源代码的依赖，可以添加到 SourceYardExcludePackageReference 列表
        public List<string> SourceYardExcludePackageReferenceList { get; private set; } = null!;

        /// <summary>
        /// 安装的源代码包列表
        /// </summary>
        /// 用于解决 https://github.com/dotnet-campus/SourceYard/issues/60
        public List<string> SourceYardPackageReferenceList { get; private set; } = null!;

        private List<string> GetList(string fileName)
        {
            var file = Path.Combine(SourcePackingDirectory, fileName);

            if (!File.Exists(file))
            {
                var sourcePackingDirectory = Directory.CreateDirectory(SourcePackingDirectory);
                _logger.Warning(
                    $"BuildProps Can not find file={file}  SourcePackingDirectory FileList={string.Join(";", sourcePackingDirectory.GetFiles().Select(temp => temp.Name))}");

                return new List<string>(0);
            }

            return File.ReadAllLines(file).Where(temp => !string.IsNullOrEmpty(temp)).ToList();
        }

        private readonly ILogger _logger;
    }
}