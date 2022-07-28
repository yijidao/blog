using System.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using MudBlazorTemplates1;
using MudBlazorTemplates1.Data;
using MudBlazorTemplates1.DB;
using MudBlazorTemplates1.Hubs;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
//builder.Services.AddSqlite<FirstDbContext>(builder.Configuration["Database:Connection"]);
builder.Services.AddSqlite<FirstDbContext>("Data Source=First.db");
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMudServices();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});

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

app.UseRouting();

app.MapBlazorHub();

app.MapHub<ChatHub>("/chathub");
//app.MapFallbackToFile("/file/{filename}", "/file/icon.jpg");
//app.MapFallbackToPage("/file/{filename}", "/_Host");

app.MapFallbackToPage("/_Host");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FirstDbContext>();
    if (!db.Database.EnsureCreated())
    {
        SeedData.Initialize(db);
    }
}


app.Run();