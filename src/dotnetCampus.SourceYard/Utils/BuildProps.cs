﻿using System.Collections.Generic;
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
            SourcePackingDirectory = packingDirectory;

            // 更多信息读取
            var sourceProjectPackageFile = Path.Combine(packingDirectory, "SourceProjectPackageFile.txt");

            // 这个文件里面存放使用 fkv 配置
            if (File.Exists(sourceProjectPackageFile))
            {
                var fileConfigurationRepo = ConfigurationFactory.FromFile(sourceProjectPackageFile);

                IAppConfigurator appConfigurator = fileConfigurationRepo.CreateAppConfigurator();
                var configuration = appConfigurator.Default;

                RepositoryType = configuration.GetValue("RepositoryType")?.Trim() ?? string.Empty;

                PackageProjectUrl = configuration.GetValue("PackageProjectUrl")?.Trim() ?? string.Empty;

                RepositoryUrl = configuration.GetValue("RepositoryUrl")?.Trim() ?? string.Empty;
            }

            const string sourceYardPackageReferenceFile = "SourceYardPackageReferenceFile.txt";
        
            SourceYardPackageReferenceList = GetList(sourceYardPackageReferenceFile);

            const string sourceYardExcludePackageReferenceFile =
                "SourceYardExcludePackageReferenceFile.txt";
            SourceYardExcludePackageReferenceList = GetList(sourceYardExcludePackageReferenceFile);
        }

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
            return File.ReadAllLines(file).Where(temp => !string.IsNullOrEmpty(temp)).ToList();
        }
    }
}