# 默认忽略的文件和文件夹

```csharp
        /// <summary>
        /// 忽略的文件夹列表
        /// </summary>
        private static IList<string> IgnoreFolderList { get; } = new List<string>()
        {
            ".vs", "bin", "obj", ".git", "x64", "x86"
        };

        /// <summary>
        /// 忽略的文件后缀列表
        /// </summary>
        private static IList<string> IgnoreFileEndList { get; } = new List<string>()
        {
            ".csproj.DotSettings", ".suo", ".user", ".sln.docstates", 
            ".nupkg",
            // 忽略原因请看 https://github.com/dotnet-campus/SourceYard/issues/98
            "launchSettings.json"
        };
```