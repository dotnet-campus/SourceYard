# 附加文件

将额外的文件加入到 SourceYard 源代码包

加入某个文件夹的所有文件的方法如下

```
  <ItemGroup>
    <SourceYardNone Include="..\dotnetCampus.CommandLine.Analyzer\bin\$(Configuration)\netstandard2.0\**" SourcePackagePath="..\analyzer\dotnet\cs\%(RecursiveDir)%(FileName).%(Extension)" />
  </ItemGroup>
```