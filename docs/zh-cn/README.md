# SourceYard 源码包

[SourceYard](about.md) 是[开源](https://github.com/dotnet-campus/SourceYard/blob/master/LICENSE)的 NuGet 源码包制作工具。它本质上也是一个 NuGet 包，当你将其安装到你的项目中后，可以为你的项目打一个不同于原生 NuGet 包的特殊包，此包仅包含你项目中的源码而没有 dll 引用。

SourceYard 适用于 .NET 项目，无论是 .NET Core 还是 .NET Framework 或者是 .NET Standard。实际上它是一个用于开发阶段使用的 NuGet 包，所以也几乎可以无视所用的平台，只要你的开发环境中具有 .NET Core 运行时即可。
