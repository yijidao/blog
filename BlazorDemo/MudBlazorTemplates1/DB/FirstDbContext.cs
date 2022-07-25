using Microsoft.EntityFrameworkCore;
using MudBlazorTemplates1.Models;

namespace MudBlazorTemplates1.DB
{
    public class FirstDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        public FirstDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
