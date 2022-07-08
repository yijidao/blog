using System.Diagnostics;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using Task = System.Threading.Tasks.Task;

namespace WorkerService1;

public class ClientService
{
    private readonly ILogger<WindowsBackgroundService> _logger;

    public ClientService(ILogger<WindowsBackgroundService> logger)
    {
        _logger = logger;
    }

    public void StartClient()
    {
        var key = Registry.LocalMachine.OpenSubKey(@"Software\PCI\HjmosClient", true);
        var pciPath = key?.GetValue("Path")?.ToString() ?? "";
        var fileName = Path.Combine(pciPath, "Hjmos.Ncc.Ws.exe");

        _logger.LogWarning($"启动 {fileName}");


        using (var ts = new TaskService())
        {

            var t = ts.Execute(fileName)
                .Once()
                .Starting(DateTime.Now.AddSeconds(2))
                .AsTask("myTask");

        }

    }

    //public Task TaskStopClient()
    //{

    //}

    public async Task WatchClient()
    {
        var key = Registry.LocalMachine.OpenSubKey(@"Software\PCI\HjmosClient", true);
        var pciPath = key?.GetValue("Path")?.ToString() ?? "";
        if (string.IsNullOrWhiteSpace(pciPath))
        {
            _logger.LogError("HjmosClientPath 为空");
        }
        else
        {
            var p= Process.GetProcessesByName("Hjmos.Ncc.WS")?.FirstOrDefault();
            await p.WaitForExitAsync();


        }
    }
}