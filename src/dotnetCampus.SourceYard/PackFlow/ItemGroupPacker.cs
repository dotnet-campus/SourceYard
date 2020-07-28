using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using dotnetCampus.SourceYard.Context;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.PackFlow
{
    internal class ItemGroupPacker : IPackFlow
    {
        /// <summary>
        /// 获取应该被加入源码引用的项类型。
        /// </summary>
        private static readonly string[] IncludingItemTypes =
        {
            // 由于需要多个项之间可能存在重合（用于 Update 和 Remove），所以 None 也是需要加入的。
            "Compile", "Resource", "Content", "None"
        };

        /// <summary>
        /// 获取应该被加入源码引用的 XAML 型的项类型。
        /// </summary>
        private static readonly string[] XamlItemTypes =
        {
            // 由于需要多个项之间可能存在重合（用于 Update 和 Remove），所以 None 也是需要加入的。
            "Page", "ApplicationDefinition"
        };

        public void Pack(IPackingContext context)
        {
            var buildAssetsFile = Path.Combine(context.PackingFolder, "build", $"{context.PackageId}.targets");

            // 从原始的项目文件中提取所有的 ItemGroup 中的节点，且节点类型在 IncludingItemTypes 中。

            // nuget 的源代码
            var sourceReferenceSourceFolder = @"$(MSBuildThisFileDirectory)..\src\";

            // 读取文件
            var buildFile = File.ReadAllText(buildAssetsFile);

            buildFile = ReplaceString(context.PackagedProjectFile, buildFile, context.PackageGuid,
                sourceReferenceSourceFolder, false,
                "<!--替换ItemGroup-->", "<!--替换XmlItemGroup-->");

            // 本地的代码，用于调试本地的代码

            sourceReferenceSourceFolder = $@"$({context.PackageGuid}SourceFolder)\";

            buildFile = ReplaceString(context.PackagedProjectFile, buildFile, context.PackageGuid,
                sourceReferenceSourceFolder, true,
                "<!--替换 SOURCE_REFERENCE ItemGroup-->", "<!--替换 SOURCE_REFERENCE XmlItemGroup-->");

            // 用户可以选择使用 nuget 源代码，也可以选择使用自己的代码，所以就需要使用两个不同的值

            // 写入文件
            File.WriteAllText(buildAssetsFile, buildFile);
        }

        private string ReplaceString(PackagedProjectFile contextPackagedProjectFile, string str,
            string packageGuid, string filePath, bool isVisible,
            string replaceItemGroup, string replaceXmlItemGroup)
        {
            var groupElement =
                new ItemGroupElement(contextPackagedProjectFile, filePath, isVisible, packageGuid);
            var (itemGroupElement, itemGroupElementOfXaml) = groupElement.GetItemGroup();

            return str.Replace(replaceItemGroup, itemGroupElement.ToString())
                .Replace(replaceXmlItemGroup, itemGroupElementOfXaml.ToString());
        }
    }
}