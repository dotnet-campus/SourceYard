# SourceYard 源码包

SourceYard 提供制作源代码的 Nuget 包的方法，制作出来的源代码包可以被安装到任意的源代码兼容的项目。最后会合并到被安装的项目的程序集。

通过 SourceYard 可以分发简单的工具库，同时可以有效减少 DLL 的数量从而提升应用程序启动性能。因为 SourceYard 制作的是源代码包，制作出来的包具备良好的兼容性，通过宏等方法可以做到在各个平台使用相同的 Nuget 库。

## 构建状态

Appveyor|Codecov
:-:|:-:
[![Build status][ai]][al]|[![codecov][ci]][cl]

<!-- a / c 是所用插件的首字母，i 是 icon，l 是 link。 -->

[ai]: https://ci.appveyor.com/api/projects/status/kxn9iakcittmvrcj?svg=true
[al]: https://ci.appveyor.com/project/xinyuehtx/sourceyard
[ci]: https://codecov.io/gh/dotnet-campus/SourceYard/branch/master/graph/badge.svg
[cl]: https://codecov.io/gh/dotnet-campus/SourceYard

## NuGet 包

[![](https://img.shields.io/nuget/v/dotnetCampus.SourceYard.svg)](https://www.nuget.org/packages/dotnetCampus.SourceYard)

## 快速入门

在项目中使用 NuGet 安装 [SourceYard](https://www.nuget.org/packages/dotnetCampus.SourceYard) 完成之后的每次编译生成，都可以在输出文件夹找到生成的 xx.source.1.0.0.nupkg 源代码包文件

将源代码包文件提交到 NuGet 源上，可以将此源代码包作为和普通的 NuGet 包一样在其他项目安装使用

以下是一个例子

创建一个空白的库程序

```
dotnet new console -o Foo
```

在创建的项目里面安装 SourceYard 库

```
cd Foo
dotnet add package dotnetCampus.SourceYard --version  0.1.7213-alpha
```

运行 dotnet build 命令进行编译，编译完成可以在 `bin\debug` 文件夹可以找到打包的源代码包

## 文档

[中文文档](./docs/zh-cn)

## 例子

[例子代码](./sample)

## 开源社区

## 规划
