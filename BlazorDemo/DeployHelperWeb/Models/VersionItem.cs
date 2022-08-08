namespace DeployHelperWeb.Models
{
    public class VersionItem
    {
        public string VersionNunber { get; set; }

        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public long Length { get; set; }

        public Guid Id { get; set; }
    }
}
