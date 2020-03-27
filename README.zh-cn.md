# SourceYard 源码包

| Build | NuGet |
|--|--|
|![](https://github.com/dotnet-campus/SourceYard/workflows/.NET%20Core/badge.svg)|[![](https://img.shields.io/nuget/v/dotnetCampus.SourceYard.svg)](https://www.nuget.org/packages/dotnetCampus.SourceYard)|

SourceYard 提供制作源代码的 Nuget 包的方法，制作出来的源代码包可以被安装到任意的源代码兼容的项目。最后会合并到被安装的项目的程序集。

通过 SourceYard 可以分发简单的工具库，同时可以有效减少 DLL 的数量从而提升应用程序启动性能。因为 SourceYard 制作的是源代码包，制作出来的包具备良好的兼容性，通过宏等方法可以做到在各个平台使用相同的 Nuget 库。

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

如果你希望参与贡献，欢迎 [Pull Request](https://github.com/dotnet-campus/SourceYard/pulls)，或给我们 [报告 Bug](https://github.com/dotnet-campus/SourceYard/issues/new)

## 规划

查看[这个页面](https://github.com/dotnet-campus/SourceYard/projects/1) 来了解我们 2020 年的开发计划

## 授权协议

[![](https://img.shields.io/badge/License-MIT-blue?style=flat-square)](https://github.com/dotnet-campus/SourceYard/blob/master/LICENSE)