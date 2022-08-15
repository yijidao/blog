using DeployHelperWeb.Service;
using MudBlazor.Services;
using DeployHelperWeb.DB;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSqlite<VersionDbContext>(builder.Configuration.GetConnectionString("Sqlite"));
//builder.Services.AddSqlite<VersionDbContext>("Data Source=Versoin.db");

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