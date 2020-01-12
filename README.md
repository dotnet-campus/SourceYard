<!-- Icon here -->

# dotnetCampus.SourceYard

Add a NuGet package only for dll reference? By using dotnetCampus.SourceYard, you can pack a NuGet package with source code. By installing the new source code package, all source codes behaviors just like it is in your project.

## Build Status

Appveyor|Codecov
:-:|:-:
[![Build status][ai]][al]|[![codecov][ci]][cl]

<!-- a and c are the first letter of CI plugins. i is icon and l is link. -->

[ai]: https://ci.appveyor.com/api/projects/status/kxn9iakcittmvrcj?svg=true
[al]: https://ci.appveyor.com/project/xinyuehtx/sourceyard
[ci]: https://codecov.io/gh/dotnet-campus/SourceYard/branch/master/graph/badge.svg
[cl]: https://codecov.io/gh/dotnet-campus/SourceYard

## NuGet Package

[![](https://img.shields.io/nuget/v/dotnetCampus.SourceYard.svg)](https://www.nuget.org/packages/dotnetCampus.SourceYard)

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

## Documentation

## Contributing

## Roadmap
