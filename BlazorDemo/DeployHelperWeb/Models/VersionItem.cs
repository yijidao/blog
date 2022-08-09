namespace DeployHelperWeb.Models
{
    public class VersionItem
    {
        public string VersionNumber { get; set; }

        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public long Length { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Creator { get; set; }

        public string Path { get; set; }
    }
}
