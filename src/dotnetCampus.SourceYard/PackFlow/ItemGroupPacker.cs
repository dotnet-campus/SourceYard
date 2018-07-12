using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using dotnetCampus.SourceYard.Context;

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
            var projectFile = context.ProjectFile;
            var buildAssetsFile = Path.Combine(context.PackingFolder, "build", $"{context.PackageId}.props");

            // 从原始的项目文件中提取所有的 ItemGroup 中的节点，且节点类型在 IncludingItemTypes 中。
            var itemsFromProject = ReadProjectItems(projectFile);
            var itemGroup = itemsFromProject.Where(x => IncludingItemTypes.Contains(x.Name));
            var itemGroupOfXaml = itemsFromProject.Where(x => XamlItemTypes.Contains(x.Name));

            // 将提取出来的节点转换成 XElement，以便随后写入 XML 文件。
            var itemGroupElement = new XElement("ItemGroup",
                itemGroup.Select(x => ConvertItemElement(XElement.Parse(x.OuterXml))));
            var itemGroupElementOfXaml = new XElement("ItemGroup",
                itemGroupOfXaml.Select(x => ConvertItemElement(XElement.Parse(x.OuterXml))));

            // 试图从临时路径读取 props 文件，如果读不到，则生成一个。
            var (outputProps, outputItemGroup, outputItemGroupOfXaml) =
                ReadOrCreatePropsFile(buildAssetsFile, context.PackageGuid, context.Logger);
            outputItemGroup.Add(itemGroupElement);
            outputItemGroupOfXaml.Add(itemGroupElementOfXaml);

            // 将新生成的 XML 文件写回 props/targets 文件。
            using (var stream = new FileInfo(buildAssetsFile).OpenWrite())
            {
                var document = new XDocument();
                document.Add(outputProps);
                document.Save(stream);
            }
        }

        private static XElement ConvertItemElement(XElement item)
        {
            RemoveAllNamespaces(item);
            foreach (var attribute in new[]
            {
                item.Attribute("Include"),
                item.Attribute("Update"),
                item.Attribute("Remove")
            }.Where(x => x != null))
            {
                var path = attribute.Value;
                attribute.SetValue($@"$(MSBuildThisFileDirectory)..\src\{path}");
            }

            item.SetAttributeValue("Visible", "False");

            return item;
        }

        private static void RemoveAllNamespaces(XElement element)
        {
            element.Attributes().Where(e => e.IsNamespaceDeclaration).Remove();
            element.Name = element.Name.LocalName;

            foreach (var node in element.DescendantNodes())
            {
                if (node is XElement xElement)
                {
                    RemoveAllNamespaces(xElement);
                }
            }
        }

        [Pure]
        private static List<XPathNavigator> ReadProjectItems(string projectFile)
        {
            List<XPathNavigator> itemGroup;

            using (var fileStream = new FileInfo(projectFile).OpenRead())
            {
                var document = new XPathDocument(fileStream);
                var navigator = document.CreateNavigator();
                itemGroup = navigator.Select("/Project/ItemGroup").OfType<XPathNavigator>()
                    .SelectMany(x => x.SelectChildren(XPathNodeType.Element).OfType<XPathNavigator>()).ToList();
                if (itemGroup.Count <= 0)
                {
                    navigator.MoveToNamespace("");
                    var @namespace = new XmlNamespaceManager(new NameTable());
                    @namespace.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");
                    itemGroup = navigator.Select("/x:Project/x:ItemGroup", @namespace).OfType<XPathNavigator>()
                        .SelectMany(x => x.SelectChildren(XPathNodeType.Element).OfType<XPathNavigator>()).ToList();
                }
            }

            return itemGroup;
        }

        /// <summary>
        /// 读取 csproj/props/targets 文件，然后返回 Project 根节点。
        /// </summary>
        [Pure]
        private static (XElement root, XElement item, XElement xaml) ReadOrCreatePropsFile(string propsFile,
            string packageGuid, ILogger log)
        {
            var extension = Path.GetExtension(propsFile);

            if (!File.Exists(propsFile))
            {
                if (extension == ".props")
                {
                    var root = new XElement("Project");
                    return (root, root, root);
                }

                if (extension == ".targets")
                {
                    var target = new XElement("Target",
                        new XAttribute("Name", $"_{packageGuid}IncludeSourceCodes"),
                        new XAttribute("BeforeTargets", "CoreCompile")
                    );
                    var xamlTarget = new XElement("Target",
                        new XAttribute("Name", $"_{packageGuid}IncludeXamlCodes"),
                        new XAttribute("BeforeTargets", "XamlPreCompile")
                    );
                    var root = new XElement("Project", target, xamlTarget);
                    return (root, target, xamlTarget);
                }

                throw new NotSupportedException("仅支持为 props 和 targets 文件添加编译文件。");
            }

            using (var stream = new FileInfo(propsFile).OpenRead())
            {
                var root = XElement.Load(stream);

                XElement item;
                XElement xaml;
                if (extension == ".props")
                {
                    item = root;
                    xaml = root;
                }
                else if (extension == ".targets")
                {
                    item = root.XPathSelectElement($"Target[@Name='_{packageGuid}IncludeSourceCodes']");
                    xaml = root.XPathSelectElement($"Target[@Name='_{packageGuid}IncludeXamlCodes']");
                    if (item == null || xaml == null)
                    {
                        log.Error($"{Path.GetFileName(propsFile)} 中需要有一个用于引入源码的 Targets");
                    }
                }
                else
                {
                    throw new NotSupportedException("仅支持为 props 和 targets 文件添加编译文件。");
                }

                return (root, item, xaml);
            }
        }
    }
}
