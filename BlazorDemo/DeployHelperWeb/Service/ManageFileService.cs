using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeployHelperWeb.Service
{
    public class ManageFileService
    {
        private readonly string _filePath;

        public ManageFileService(IConfiguration configuration)
        {
            _filePath = configuration["FilePath"] ?? string.Empty;
        }

        public List<VersionDetail> GetVersionDetails()
        {
            var files = Directory.CreateDirectory(_filePath).GetFiles("*.zip");

            foreach (var fileInfo in files)
            {
                var versionDetail = new VersionDetail
                {
                    Name = fileInfo.Name,
                    CreateTime = fileInfo.CreationTime,
                    Size = GetSize(fileInfo.Length),
                };

                using var zf = ZipFile.OpenRead(fileInfo.FullName);

                var versionDescription = zf.Entries.FirstOrDefault(x => x.Name == "VersionDescription");
                if (versionDescription != null)
                {
                    using var reader = new StreamReader(versionDescription.Open());
                    versionDetail.Description = reader.ReadToEnd();
                }

            }


            var versionDetails = files.Select(x => new VersionDetail
            {
                Name = x.Name,
                CreateTime = x.CreationTime,
                Size = GetSize(x.Length),
            }).ToList();


            return versionDetails;
        }


        private string GetSize(long length) =>
            length switch
            {
                >= 1 << 30 => $"{length >> 30}GB",
                >= 1 << 20 => $"{length >> 20}MB",
                >= 1 << 10 => $"{length >> 10}KB",
                _ => length.ToString()
            };
    }

    public class VersionDetail
    {
        public string Name { get; set; } = string.Empty;

        public DateTime? CreateTime { get; set; }

        public string Description { get; set; } = string.Empty;


        public string Size { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;
    }
}
