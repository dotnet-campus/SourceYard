namespace dotnetCampus.SourceYard.Utils
{
    public class BuildProps
    {
        /// <summary>
        ///     公司
        /// </summary>
        public string Company
        {
            get
            {
                if (string.IsNullOrEmpty(_company))
                {
                    _company = "dotnet campus";
                }

                return _company;
            }
            set => _company = value;
        }

        /// <summary>
        ///     作者
        /// </summary>
        public string Authors
        {
            get
            {
                if (string.IsNullOrEmpty(_authors))
                {
                    _authors = Owner;
                }

                return _authors;
            }
            set => _authors = value;
        }

        /// <summary>
        ///     仓库地址
        /// </summary>
        public string RepositoryUrl { get; set; }

        public string RepositoryType { get; set; }

        /// <summary>
        ///     项目地址
        /// </summary>
        public string PackageProjectUrl { get; set; }

        /// <summary>
        ///     版权
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        ///     描述
        /// </summary>
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    _description = "This package is built by SourceYard";
                }

                return _description;
            }
            set => _description = value;
        }

        public string Owner
        {
            get
            {
                if (string.IsNullOrEmpty(_owner))
                {
                    _owner = "dotnet campus";
                }

                return _owner;
            }
            set => _owner = value;
        }

        public string Title { get; set; }

        public string PackageLicenseUrl { get; set; }

        public string PackageIconUrl { get; set; }

        public string PackageReleaseNotes { get; set; }

        public string PackageTags { get; set; }

        private string _authors;
        private string _company;

        private string _description;
        private string _owner;
    }
}