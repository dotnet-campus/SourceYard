# 引用源代码包依赖的原理

在 `$(PackageId).props` 文件，也就是会在打包的时候，放在打出的源代码包，作为对应的源代码包的 Build 文件夹里面控制构建的 props 文件，在这个文件里面将当前源代码包的包 Id 加入到 SourceYardPackageReference 属性里面，如下面代码

```xml
    <PropertyGroup>
        <!-- 下面代码用来引用源代码包依赖 -->
        <SourceYardPackageReference>$(SourceYardPackageReference);#(PackageId)</SourceYardPackageReference>
    </PropertyGroup>
```

这样在其他的项目，安装了源代码包，就可以通过 SourceYardPackageReference 属性了解当前项目安装了哪些源代码包

例如有项目 A 安装了 B 源代码包，那么通过 SourceYardPackageReference 属性就能拿到 B 源代码包

在将安装了源代码包的项目打包为源代码包时，例如将上面的 A 项目打包为源代码包时，想要添加 B 源代码包的引用依赖，就可以通过 SourceYardPackageReference 属性拿到当前 A 项目安装的源代码包，将这些源代码加入到依赖中

添加源代码包依赖的逻辑放在 Core.targets 里，将 SourceYardPackageReference 属性写入到 SourceYardPackageReferenceFile 文件里面

```xml
    <WriteLinesToFile File="$(SourceYardPackageReferenceFile)" Lines="$(SourceYardPackageReference)" Overwrite="true"></WriteLinesToFile>
```

这个 SourceYardPackageReferenceFile 文件将会在 BuildProps 里面读取然后加入引用依赖