namespace dotnetCampus.SourceYard.Context
{
    /// <summary>
    /// 被打包项目的文件，包括编译的文件资源文件
    /// </summary>
    class PackagedProjectFile
    {
        /// <summary>
        /// 需要做源码包的项目的编译的文件
        /// </summary>
        public string CompileFile { get; }

        /// <summary>
        /// 需要做源码包的项目的资源文件
        /// </summary>
        public string ResourceFile { get; }

        /// <summary>
        /// 需要做源码包项目的文件
        /// </summary>
        public string ContentFile { get; }

        /// <summary>
        /// 需要做源码包项目的页面
        /// </summary>
        public string Page { get; }

        /// <summary>
        /// 嵌入文件
        /// </summary>
        public string EmbeddedResource { get;}

        /// <summary>
        /// 需要做源码包项目的文件
        /// </summary>
        private string ApplicationDefinition { get; }

        public string NoneFile { get; }

        /// <inheritdoc />
        public PackagedProjectFile(string compileFile, string resourceFile, string contentFile, string page, string applicationDefinition, string noneFile, string embeddedResource)
        {
            CompileFile = compileFile;
            ResourceFile = resourceFile;
            ContentFile = contentFile;
            Page = page;
            ApplicationDefinition = applicationDefinition;
            NoneFile = noneFile;
            EmbeddedResource = embeddedResource;
        }
    }
}