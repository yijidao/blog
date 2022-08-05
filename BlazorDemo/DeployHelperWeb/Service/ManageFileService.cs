namespace DeployHelperWeb.Service
{
    public class ManageFileService
    {
        public string FilePath { get; set; }

        public ManageFileService(IConfiguration configuration)
        {
            FilePath = configuration["FilePath"] ?? string.Empty;

            var files = Directory.CreateDirectory(FilePath).GetFiles("*.zip");

            var versionDetails = files.Select(x => new VersionDetail
            {
                Name = x.Name,
                CreateTime = x.CreationTime,
                Size = GetSize(x.Length),

            });

        }


        private string GetSize(long length) =>
            length switch
            {
                >= 1<<30 => $"{length << 30}GB",
                >= 1<<20 => $"{length << 20}MB",
                >= 1<<10 => $"{length << 10}KB",
                _ => length.ToString()
            };
    }

    public class VersionDetail
    {
        public string Name { get; set; }

        public DateTime CreateTime { get; set; }

        public string Description { get; set; }


        public string Size { get; set; }

        public string Version { get; set; }

    }
}
