using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.Context
{
    /// <summary>
    /// 包含对项目进行源码打包所需的上下文信息。
    /// </summary>
    internal interface IPackingContext
    {
        /// <summary>
        /// 使用 <see cref="ILogger"/> 在编译过程记录日志，其中的错误和警告将带来编译错误和编译警告。
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// 执行打包核心代码的此程序的项目文件路径，即 dotnetCampus.SourceYard 项目的绝对路径。
        /// </summary>
        string SelfProjectFile { get; }

        /// <summary>
        /// 执行打包核心代码的此程序的项目文件所在的文件夹，即 dotnetCampus.SourceYard 项目所在的文件夹。
        /// </summary>
        string SelfProjectFolder { get; }

        /// <summary>
        /// 需要进行源码打包的项目文件。
        /// </summary>
        string ProjectFile { get; }

        /// <summary>
        /// 需要进行源码打包的项目所在的文件夹。
        /// </summary>
        string ProjectFolder { get; }

        /// <summary>
        /// 需要进行源码打包的项目的项目名称。
        /// </summary>
        string ProjectName { get; }

        /// <summary>
        /// 需要进行源码打包的项目的包 Id。
        /// </summary>
        string PackageId { get; }

        /// <summary>
        /// 需要进行源码打包的项目的全局唯一标识符。
        /// </summary>
        string PackageGuid { get; set; }

        /// <summary>
        /// 源码包的包版本。
        /// </summary>
        string PackageVersion { get; }

        /// <summary>
        /// 源码包（nupkg）文件应该输出到此路径所在的文件夹下。
        /// </summary>
        string PackageOutputPath { get; }

        /// <summary>
        /// 为了打源码包可以使用的打包临时文件夹。NuGet 包会按照此文件夹的文件夹结构来生成 nupkg 文件。
        /// </summary>
        string PackingFolder { get; }

        BuildProps BuildProps { get; }
    }
}
