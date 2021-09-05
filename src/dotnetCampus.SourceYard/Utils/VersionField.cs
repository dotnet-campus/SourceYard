// 请注意：本文件仅为提高 DEBUG 下编译速度而写，请勿开放到 Release 配置下使用！
// 如果将来有语义版本号需要，可以考虑引用 NuGet 包。
// SemanticVersion，语义版本号，MIT 开源协议
// 项目地址：https://github.com/GitTools/GitVersion/blob/master/src/GitVersionCore/SemanticVersion.cs

namespace dotnetCampus.SourceYard.Utils
{
    public enum VersionField
    {
        None,
        Patch,
        Minor,
        Major
    }
}
