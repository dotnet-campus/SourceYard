using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace dotnetCampus.SourceYard.Utils
{
    class ItemGroupElement
    {
        /// <inheritdoc />
        public ItemGroupElement(PackagedProjectFile contextPackagedProjectFile, string filePath, bool isVisible)
        {
            _contextPackagedProjectFile = contextPackagedProjectFile;
            _filePath = filePath;
            _isVisible = isVisible;
        }

        public (XElement itemGroupElement, XElement itemGroupElementOfXaml) GetItemGroup()
        {
            var contextPackagedProjectFile = _contextPackagedProjectFile;
            var compileFileList = GetFileList(contextPackagedProjectFile.CompileFile);
            var contentFileList = GetFileList(contextPackagedProjectFile.ContentFile);
            var resourceFileList = GetFileList(contextPackagedProjectFile.ResourceFile);
            var noneFileList = GetFileList(contextPackagedProjectFile.NoneFile);
            var embeddedResource = GetFileList(contextPackagedProjectFile.EmbeddedResource);
            var pageFileList = GetFileList(contextPackagedProjectFile.Page);

            var elementList = new List<XElement>();
            elementList.AddRange(IncludingItemCompileFileToElement(compileFileList, "Compile", false));
            elementList.AddRange(IncludingItemCompileFileToElement(contentFileList, "Resource", true));
            elementList.AddRange(IncludingItemCompileFileToElement(resourceFileList, "Content", true));
            elementList.AddRange(IncludingItemCompileFileToElement(embeddedResource, "EmbeddedResource", true));
            elementList.AddRange(IncludingItemCompileFileToElement(noneFileList, "None", true));

            var itemGroupElement = new XElement("ItemGroup", elementList);

            elementList = new List<XElement>();
            elementList.AddRange(XamlItemCompileFileToElement(pageFileList, "Page", false));

            var itemGroupElementOfXaml = new XElement("ItemGroup", elementList);

            return (itemGroupElement, itemGroupElementOfXaml);
        }

        private readonly PackagedProjectFile _contextPackagedProjectFile;
        private readonly string _filePath;
        private readonly bool _isVisible;

        private List<XElement> XamlItemCompileFileToElement(List<string> compileFileList, string includingItemTypes,
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

        private List<XElement> IncludingItemCompileFileToElement(List<string> compileFileList,
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

        private List<string> GetFileList(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                return new List<string>();
            }

            var fileList = File.ReadAllLines(file).ToList();

            fileList = RemoveTempFile(fileList);

            return fileList;
        }

        private List<string> RemoveTempFile(List<string> fileList)
        {
            fileList.RemoveAll
            (
                temp => temp.StartsWith("obj\\")
                        || temp.StartsWith("bin\\")
            );

            fileList.RemoveAll(temp =>
            {
                var pathRoot = Path.GetPathRoot(temp);
                if (!string.IsNullOrEmpty(pathRoot))
                {
                    return temp.StartsWith(pathRoot);
                }

                return false;
            });

            return fileList;
        }
    }
}