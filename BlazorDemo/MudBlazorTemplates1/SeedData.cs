using MudBlazorTemplates1.DB;
using MudBlazorTemplates1.Models;

namespace MudBlazorTemplates1;

public class SeedData
{
    public static void Initialize(FirstDbContext firstDbContext)
    {
        if (firstDbContext.Employees.Any()) return;
        var employees = new Employee[]
        {
            new Employee { Name = "1号" },
            new Employee { Name = "2号" },
            new Employee {Name = "3号" },
        };
        firstDbContext.Employees.AddRange(employees);
        firstDbContext.SaveChanges();
    }
}