using System;
using dotnetCampus.SourceYard.Utils;

namespace dotnetCampus.SourceYard
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //// 条件
            //var version = "[2.1.0.293,3.0)";

            ////判断
            //AssertVersion(ReferenceVersion.Parser(version), new ReferenceVersion(new Version("2.1.0.293"), new Version("3.0"), true, false));

            //version = "[1.1.0.34,2.0)";

            //AssertVersion(ReferenceVersion.Parser(version), new ReferenceVersion(new Version("1.1.0.34"), new Version("2.0"), true, false));

            //version = "[4.4.0,)";

            //AssertVersion(ReferenceVersion.Parser(version), new ReferenceVersion(new Version("4.4.0"), null, true, false));

            //version = "(1.1.0.34,2.0]";

            //AssertVersion(ReferenceVersion.Parser(version), new ReferenceVersion(new Version("1.1.0.34"), new Version("2.0"), false, true));

            //version = "5.2";

            //AssertVersion(ReferenceVersion.Parser(version), new ReferenceVersion(new Version("5.2")));

            //var str = CsprojToNuspecFile.GetPackageVersion(ReferenceVersion.Parser("(1.1.0.34,2.0]"));

            //str = CsprojToNuspecFile.GetPackageVersion(ReferenceVersion.Parser("[2.1.0.293,3.0)"));

            //str = CsprojToNuspecFile.GetPackageVersion(ReferenceVersion.Parser("5.2"));


            try
            {
                var selfProjectFile = args[1];
                var intermediateOutputPath = args[3];
                var packageOutputPath = args[5];
                var packageVersion = args[7];
                var packingProjects = args[9].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                new Packer(selfProjectFile, intermediateOutputPath, packageOutputPath, packageVersion, packingProjects).Pack();
            }
            catch (Exception e)
            {
                new Logger.Logger().Error(e.Message);
            }
        }

        private static void AssertVersion(ReferenceVersion version1, ReferenceVersion version2)
        {
            if
            (
                version1.IsIncludeMinVersion == version2.IsIncludeMinVersion
                && version1.IsIncludeMaxVersion == version2.IsIncludeMaxVersion
                && version1.MinVersion == version2.MinVersion
                && version1.MaxVersion == version2.MaxVersion
                && version1.Version == version2.Version
            )
            {
            }
            else
            {
            }
        }
    }
}