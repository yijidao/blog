using DeployHelperWeb.Service;
using MudBlazor.Services;
using DeployHelperWeb.DB;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddSqlite<VersionDbContext>(builder.Configuration.GetConnectionString("Sqlite"));

var serverVersion = new MySqlServerVersion(new Version(8, 0, 30));
builder.Services.AddDbContext<VersionDbContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(builder.Configuration.GetConnectionString("MysqlConnect"), serverVersion)
        // The following three options help with debugging, but should
        // be changed or removed for production.
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();

builder.Services.AddMudServices();
builder.Services.AddSingleton<ManageFileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

var dir = builder.Configuration["FilePath"];
if (!Directory.Exists(dir))
{
    Directory.CreateDirectory(dir);
}

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(builder.Configuration["FilePath"]),
    RequestPath = "/Download",
    EnableDirectoryBrowsing = true
});

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");



app.Run();