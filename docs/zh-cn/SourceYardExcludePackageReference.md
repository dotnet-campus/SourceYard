# SourceYardExcludePackageReference

排除的依赖引用

在打源代码包的时候，有一些 NuGet 引用依赖不期望被加入源代码的依赖，以提升源代码包的兼容性

此时可以采用 SourceYardExcludePackageReference 机制，在 SourceYardExcludePackageReference 项里面添加的 NuGet 包 Id 内容，将不会被加入源代码包引用依赖

例如有项目 A 引用了两个 NuGet 包，分别是 B 和 C 库

```xml
  <ItemGroup>
    <PackageReference Include="B" Version="1.0.0" />
    <PackageReference Include="C" Version="1.0.0" />
  </ItemGroup>
```

而项目 A 不期望打出的源代码包，包含了 B 和 C 库的依赖。可以在项目文件里面添加 SourceYardExcludePackageReference 列表，如下面代码

```xml
  <ItemGroup>
    <SourceYardExcludePackageReference Include="B" />
    <SourceYardExcludePackageReference Include="C" />
  </ItemGroup>
```

此时打包出来的源代码包将不会包含 B 和 C 库的依赖