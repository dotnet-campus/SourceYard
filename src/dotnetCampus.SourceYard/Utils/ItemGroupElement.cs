using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using dotnetCampus.SourceYard.Context;

namespace dotnetCampus.SourceYard.Utils
{
    class ItemGroupElement
    {
        /// <inheritdoc />
        public ItemGroupElement(PackagedProjectFile contextPackagedProjectFile, string filePath, bool isVisible,
            string packageGuid)
        {
            _contextPackagedProjectFile = contextPackagedProjectFile;
            _filePath = filePath;
            _isVisible = isVisible;
            _packageGuid = packageGuid;
        }

        public (XElement itemGroupElement, XElement itemGroupElementOfXaml) GetItemGroup()
        {
            var contextPackagedProjectFile = _contextPackagedProjectFile;
            var compileFileList = contextPackagedProjectFile.CompileFileList;
            var contentFileList = contextPackagedProjectFile.ContentFileList;
            var resourceFileList = contextPackagedProjectFile.ResourceFileList;
            var noneFileList = contextPackagedProjectFile.NoneFileList;
            var embeddedResource = contextPackagedProjectFile.EmbeddedResourceList;
            var pageFileList = contextPackagedProjectFile.PageList;

            var prefix = $"_{_packageGuid}";

            var elementList = new List<XElement>();
            elementList.AddRange(IncludingItemCompileFileToElement(compileFileList, $"{prefix}Compile", false));
       
            elementList.AddRange(IncludingItemCompileFileToElement(contentFileList, $"Content", true));
            elementList.AddRange(IncludingItemCompileFileToElement(embeddedResource, $"EmbeddedResource", true));
            elementList.AddRange(IncludingItemCompileFileToElement(noneFileList, "None", true));

            var itemGroupElement = new XElement("ItemGroup", elementList);

            elementList = new List<XElement>();
            elementList.AddRange(XamlItemCompileFileToElement(resourceFileList, $"{prefix}Resource", true));
            elementList.AddRange(XamlItemCompileFileToElement(pageFileList, $"{prefix}Page", false));

            var itemGroupElementOfXaml = new XElement("ItemGroup", elementList);

            return (itemGroupElement, itemGroupElementOfXaml);
        }

        private readonly PackagedProjectFile _contextPackagedProjectFile;
        private readonly string _filePath;
        private readonly bool _isVisible;
        private readonly string _packageGuid;

        private List<XElement> XamlItemCompileFileToElement(IEnumerable<string> compileFileList, string includingItemTypes,
            bool copyToOutputDirectory)
        {
            var elementList = new List<XElement>();

            foreach (var temp in compileFileList)
            {
                var element = new XElement(includingItemTypes);

                var file = _filePath + temp;

                SetXmlItemElement(element, copyToOutputDirectory, file);

                elementList.Add(element);
            }

            return elementList;
        }

        private List<XElement> IncludingItemCompileFileToElement(IReadOnlyList<string> compileFileList,
            string includingItemTypes, bool copyToOutputDirectory)
        {
            var elementList = new List<XElement>();
            foreach (var temp in compileFileList)
            {
                var element = new XElement(includingItemTypes);
                var file = _filePath + temp;
                SetItemElement(element, copyToOutputDirectory, file);

                elementList.Add(element);
            }

            return elementList;
        }

        private void SetXmlItemElement(XElement element, bool copyToOutputDirectory, string file)
        {
            element.SetAttributeValue("SubType", "Designer");
            element.SetAttributeValue("Generator", "MSBuild:Compile");

            // xml 的文件需要放在 XamlPreCompile 之前也就是放在 Target 里无法编译
            // 所以需要 xml 的文件自己添加 Condition 判断当前使用本地文件

            SetItemElement(element, copyToOutputDirectory, file);
        }


        private void SetItemElement(XElement element, bool copyToOutputDirectory, string file)
        {
            element.SetAttributeValue("Include", file);
            //element.SetAttributeValue("Link", "%(RecursiveDir)%(Filename)%(Extension)");

            if (_isVisible)
            {
                element.SetAttributeValue("Visible", "True");
            }
            else
            {
                element.SetAttributeValue("Visible", "False");
            }

            if (copyToOutputDirectory)
            {
                element.SetAttributeValue("CopyToOutputDirectory", "PreserveNewest");
            }
        }
    }
}