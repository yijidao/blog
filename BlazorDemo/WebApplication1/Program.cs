using System.Diagnostics;
using WebApplication1.Options;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureServices((context, services) =>
            {
                services.Configure<HjmosClientOptions>(context.Configuration.GetSection(HjmosClientOptions.HjmosClient));
                services.AddSingleton<ClientService>();
            });


            var app = builder.Build();


            //Console.WriteLine("Hello World!");

            //Process.Start(@"C:\pci\cs6\HJMos_NCC_Client\Work\Hjmos_Client\Hjmos.Ncc.WS.exe");

            app.Services.GetService<ClientService>().Run();

            app.Run();
        }


        //public static void Main(string[] args)
        //{
        //    var builder = WebApplication.CreateBuilder(args);
        //    var app = builder.Build();

        //    app.MapGet("/", () => "Hello World!");

        //    app.Run();
        //}
    }
}