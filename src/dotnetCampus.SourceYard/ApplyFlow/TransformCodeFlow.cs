using System;
using System.IO;
using System.IO.Compression;
using dotnetCampus.SourceYard.Cli;
using dotnetCampus.SourceYard.Context;

namespace dotnetCampus.SourceYard.ApplyFlow
{
    /// <summary>
    /// 进行源代码转换。
    /// </summary>
    internal class TransformCodeFlow : IApplyFlow
    {
        public void Apply(ApplyOptions options)
        {
            if (!options.InternalAllClasses && !options.UseLocalNamespace)
            {
                Console.WriteLine(options.SourceFolder);
            }
        }
    }
}
