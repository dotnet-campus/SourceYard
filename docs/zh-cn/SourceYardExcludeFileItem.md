# SourceYardExcludeFileItem

不要在 SourceYard 里面包含的文件项，这些文件项将在打包时被排除

例如有一些文档是写入到项目里面，期望打包的时候，不要将这些文档打包添加到源代码包引用里面，以免被输出

又例如有一些调试使用的文件，例如 foo.coin 文件，不期望作为资源被其他项目引用

将不期望被源代码包引用的文件项加入到 SourceYardExcludeFileItem 列表即可在打包的时候被忽略引用，但文件依然会被添加到源代码里面，只是引用而已

加入方法如下

```xml
  <ItemGroup>
    <SourceYardExcludeFileItem Include="foo.coin" />
    <SourceYardExcludeFileItem Include="Resource\F1.md" />
  </ItemGroup>
```