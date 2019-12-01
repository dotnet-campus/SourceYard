using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dotnetCampus.SourceYard.Utils
{
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

                return _company;
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

                return _authors;
            }
            set => _authors = value;
        }

        /// <summary>
        ///     仓库地址
        /// </summary>
        /// 在 SourceProjectPackageFile.txt 设置
        public string RepositoryUrl { get; private set; }

        public string RepositoryType { get; private set; }

        /// <summary>
        ///     项目地址
        /// </summary>
        /// 在 SourceProjectPackageFile.txt 设置
        public string PackageProjectUrl { get; private set; }

        /// <summary>
        ///     版权
        /// </summary>
        public string Copyright { get; set; }

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

                return _description;
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

                return _owner;
            }
            set => _owner = value;
        }

        public string Title { get; set; }

        public string PackageLicenseUrl { get; set; }

        public string PackageIconUrl { get; set; }

        public string PackageReleaseNotes { get; set; }

        public string PackageTags { get; set; }

        /// <summary>
        /// 打包用到的文件夹
        /// </summary>
        public string SourcePackingDirectory { get; private set; }

        private string _authors;
        private string _company;

        private string _description;
        private string _owner;

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
                // todo 等待高性能配置发布
                var configList = DeserializeFile(new FileInfo(sourceProjectPackageFile));

                if (configList.TryGetValue("RepositoryType", out var repositoryType))
                {
                    RepositoryType = repositoryType.Trim();
                }

                if (configList.TryGetValue("PackageProjectUrl", out var packageProjectUrl))
                {
                    PackageProjectUrl = packageProjectUrl.Trim();
                }

                if (configList.TryGetValue("RepositoryUrl", out var repositoryUrl))
                {
                    RepositoryUrl = repositoryUrl.Trim();
                }
            }
        }

        // 这里代码从Configurations复制
        private Dictionary<string, string> DeserializeFile(FileInfo file)
        {
            // 一次性读取完的性能最好
            var str = File.ReadAllText(file.FullName);
            str = str.Replace("\r\n", "\n");
            return Deserialize(str);
        }
        private static string _splitString = ">";

        private static string _escapeString = "?";

        /// <summary>
        /// 反序列化的核心实现，反序列化字符串
        /// </summary>
        /// <param name="str"></param>
        private Dictionary<string, string> Deserialize(string str)
        {
            var keyValuePairList = str.Split('\n');
            var keyValue = new Dictionary<string, string>(StringComparer.Ordinal);
            string? key = null;
            var splitString = _splitString;

            foreach (var temp in keyValuePairList.Select(temp => temp.Trim()))
            {
                if (temp.StartsWith(splitString, StringComparison.Ordinal))
                {
                    // 分割，可以作为注释，这一行忽略
                    // 下一行必须是key
                    key = null;
                    continue;
                }

                var unescapedString = UnescapeString(temp);

                if (key == null)
                {
                    key = unescapedString;

                    // 文件存在多个地方都记录相同的值
                    // 如果有多个地方记录相同的值，使用最后的值替换前面文件
                    if (keyValue.ContainsKey(key))
                    {
                        keyValue.Remove(key);
                    }
                }
                else
                {
                    if (keyValue.ContainsKey(key))
                    {
                        // key
                        // v1
                        // v2
                        // 返回 {"key","v1\nv2"}
                        keyValue[key] = keyValue[key] + "\n" + unescapedString;
                    }
                    else
                    {
                        keyValue.Add(key, unescapedString);
                    }
                }
            }

            return keyValue;
        }

        /// <summary>
        /// 存储的反转义
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string UnescapeString(string str)
        {
            var escapeString = _escapeString;

            if (str.StartsWith(escapeString, StringComparison.Ordinal))
            {
                return str.Substring(1);
            }

            return str;
        }
    }
}