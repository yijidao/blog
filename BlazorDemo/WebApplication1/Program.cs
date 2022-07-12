using System.Diagnostics;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<ClientService>();
            });


            var app = builder.Build();


            //Console.WriteLine("Hello World!");

            //Process.Start(@"C:\pci\cs6\HJMos_NCC_Client\Work\Hjmos_Client\Hjmos.Ncc.WS.exe");

            //app.Services.GetService<ClientService>().Watch();

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