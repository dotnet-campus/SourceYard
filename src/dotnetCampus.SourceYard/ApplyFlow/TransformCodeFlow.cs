using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using dotnetCampus.SourceYard.Cli;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard.ApplyFlow
{
    /// <summary>
    /// 进行源代码转换。
    /// </summary>
    internal class TransformCodeFlow : IApplyFlow
    {
        public void Apply(ApplyOptions options)
        {
            if (!options.InternalAllClasses && string.IsNullOrWhiteSpace(options.LocalRootNamespace))
            {
                Console.WriteLine(options.SourceFolder);
            }
            else
            {
                Console.WriteLine(options.TransformedSourceFolder);
            }

            var got = false;
            string originalNamespace = null;
            foreach (var line in File.ReadAllLines(options.OriginalProjectConfigs))
            {
                if (got)
                {
                    originalNamespace = line;
                    break;
                }
                if (line == "RootNamespace")
                {
                    got = true;
                }
            }

            FileSystem.TransformFolderContents(options.SourceFolder, options.TransformedSourceFolder,
                (file, content) => TransformCode(file, content,
                    originalNamespace, options.LocalRootNamespace,
                    options.InternalAllClasses));
        }

        private string TransformCode(FileInfo file, string content,
            string originalNamespace, string rootNamespace,
            bool internalAllClasses)
        {
            var ext = file.Extension;

            if (_codeConverters.TryGetValue(ext, out var transformer))
            {
                return transformer(content, originalNamespace, rootNamespace, internalAllClasses);
            }

            return content;
        }

        private readonly Dictionary<string, Func<string, string, string, bool, string>> _codeConverters = new Dictionary<string, Func<string, string, string, bool, string>>
        {
            { ".cs", TransformCSharpFile },
            { ".xaml", TransformXamlFile },
        };

        private static readonly Regex CSharpRegex = new Regex("public ((static )?(class|interface|struct|enum|delegate))", RegexOptions.Compiled);

        private static string TransformCSharpFile(string content, string oldNamespace, string newNamespace, bool internalAllClasses)
        {
            if (internalAllClasses)
            {
                content = CSharpRegex.Replace(content, "internal $1");
            }

            if (!string.IsNullOrWhiteSpace(newNamespace))
            {
                content = content.Replace($"namespace {oldNamespace}", $"namespace {newNamespace}");
            }

            return content;
        }

        private static string TransformXamlFile(string content, string oldNamespace, string newNamespace, bool internalAllClasses)
        {
            return content;
        }
    }
}
