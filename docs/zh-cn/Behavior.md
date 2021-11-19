# 行为和配置

## 特殊配置

在 SourceYard 打出的源代码包中，可以通过一些特殊的配置实现源代码包特殊的功能

### DisableSourcePackageAutoPrivateAssets

禁止自动配置目标全部源代码包自动设置 PrivateAssets 属性，默认的源代码将会自动添加如下代码

```xml
  <ItemGroup Condition="$(DisableSourcePackageAutoPrivateAssets) != 'true'">
    <!-- 用于修复设置了 DevelopmentDependency 将不自动添加依赖 -->
    <!-- 参阅 https://github.com/dotnet-campus/SourceYard/issues/112 -->
    <PackageReference Update="#(PackageId)">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>all</IncludeAssets>
    </PackageReference>
  </ItemGroup>
```

可以通过设置 DisableSourcePackageAutoPrivateAssets 属性为 true 禁用当前项目全部安装的源代码包自动设置

以上的 `#(PackageId)` 就是对应的安装的源代码包，如 `TheLib.Source` 等

如需要对具体某个源代码包进行配置，请自行设置更新覆盖即可