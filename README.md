# dotnetCampus.SourceYard

| Build | NuGet |
|--|--|
|![](https://github.com/dotnet-campus/SourceYard/workflows/.NET%20Core/badge.svg)|[![](https://img.shields.io/nuget/v/dotnetCampus.SourceYard.svg)](https://www.nuget.org/packages/dotnetCampus.SourceYard)|

Add a NuGet package only for dll reference? By using dotnetCampus.SourceYard, you can pack a NuGet package with source code. By installing the new source code package, all source codes behaviors just like it is in your project.

## Getting Started

### Packing a source NuGet package

Using NuGet tool to install [dotnetCampus.SourceYard](https://www.nuget.org/packages/dotnetCampus.SourceYard) to your project

```
PM> Install-Package dotnetCampus.SourceYard 
```

Or

```
dotnet add package dotnetCampus.SourceYard
```

And then you should build your project and you will find the `*.Source.version.nupkg` file in your package output folder

**Remark**

The package with 0.1.19035-alpha version will break the double package reference project.

If you want to support the double package reference project, you can use the package with 0.1.19033-alpha version.

## Documentation

[简体中文](README.zh-cn.md)

[NuGet shared source packages Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/dependencies#nuget-shared-source-packages )

## Contributing

[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](https://github.com/dotnet-campus/SourceYard/pulls)

If you would like to contribute, feel free to create a [Pull Request](https://github.com/dotnet-campus/SourceYard/pulls), or give us [Bug Report](https://github.com/dotnet-campus/SourceYard/issues/new).

## Roadmap

Check out this [Roadmap](https://github.com/dotnet-campus/SourceYard/projects/1) to learn about our development plans for 2020.

## Thanks

- [@Zhuangkh](https://github.com/Zhuangkh)