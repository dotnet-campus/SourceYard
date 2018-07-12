# 此文件夹结构

此 Assets 目录有两个独立的文件夹，`Current` 和 `Target`。

- `Current`
    - 组织 dotnetCampus.SourceYard 的 NuGet 目录结构。
    - 此文件夹中的所有内容将会原封不动地打包到 dotnetCampus.SourceYard 的 NuGet 包中。
- `Target`
    - 组织安装此 NuGet 包的目标项目的 NuGet 目录结构。
    - 例如，Foo 项目安装了此 dotnetCampus.SourceYard 包；那么，当 Foo 项目打包时，此文件夹中的内容将打包到 Foo 的 NuGet 包中。
    - 此文件夹中的所有内容可以包含目标包的占位符，为了与 MSBuild 原生的占位符前缀 `$`, `@`, `%` 区分，此包使用 `#` 作为占位符前缀。

对于以上每一个 NuGet 目录，都遵循通用的 NuGet 包的文件夹结构：

+ `/`
    - 根目录，用来放 readme.txt 的（已经有人提 issue 要求加入 markdown 支持了）
+ `lib/`
    - 用来放引用程序集 .dll，文档注释 .xml 和符号文件 .pdb 的
+ `runtimes/`
    - 用来放那些与平台相关的 .dll/.pdb/.pri 的
+ `content/`
    - 任意种类的文件，在这个文件夹中的文件会在编译时拷贝到输出目录（保持文件夹结构）
+ `build/`
    - 这里放 .props 和 .targets 文件，会自动被 NuGet 导入，成为项目的一部分（要求文件名与包名相同）
+ `buildCrossTargeting/`
    - 这里也是放 .props 和 .targets 文件，会自动被 NuGet 导入，成为项目的一部分（要求文件名与包名相同）
+ `tools/`
    - PowerShell 脚本或者程序，在这里的工具可以在“包管理控制台”(Package Manager Console) 中使用
