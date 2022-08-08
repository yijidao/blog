using DeployHelperWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace DeployHelperWeb.DB
{
    public class VersionDbContext : DbContext
    {
        public DbSet<VersionItem> VersionItems { get; set; }

        public VersionDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
