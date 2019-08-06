using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace dotnetCampus.SourceYard.Cli
{
    [Verb("apply")]
    class ApplyOptions
    {
        /// <summary>
        /// 源代码所在的路径
        /// </summary>
        [Option('s', "source", Required = true)]
        public string SourceFolder { get; set; }

        /// <summary>
        /// 转换后的源代码所在的路径
        /// </summary>
        [Option('t', "transform", Required = true)]
        public string TransformedSourceFolder { get; set; }

        /// <summary>
        /// 源项目中已经存储的部分配置
        /// </summary>
        [Option('c', "configs", Required = true)]
        public string OriginalProjectConfigs { get; set; }

        /// <summary>
        /// 是否需要将命名空间修改为本地命名空间。
        /// </summary>
        [Option("local", Required = true)]
        public string LocalRootNamespaceCore
        {
            get => LocalRootNamespace;
            set => LocalRootNamespace = value?.Equals("null", StringComparison.OrdinalIgnoreCase) == false ? value : null;
        }

        public string LocalRootNamespace { get; private set; }

        /// <summary>
        /// 是否需要将所有的类型改为 internal。
        /// </summary>
        [Option("internal", Required = true)]
        public string InternalAllClassesCore
        {
            get => InternalAllClasses.ToString();
            set => InternalAllClasses = value?.Equals("true", StringComparison.OrdinalIgnoreCase) is true;
        }

        /// <summary>
        /// 是否需要将所有的类型改为 internal。
        /// </summary>
        public bool InternalAllClasses { get; private set; }
    }
}
